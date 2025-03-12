using AutoMapper;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers

{
    [ApiController]
    [Route("api/books")]
    //[Authorize]//protegemos a nivel de controllador el acceso a sus endpoints
    [Authorize(Policy = "isadmin")]//protegemos a nivel de controllador el acceso a sus endpoints--solo el admin puede usar estos metodos
    public class BooksController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IEnumerable<BookDTO>> Get()
        {
            //return await context.Books.Include(x => x.Author).ToListAsync();
            var books = await context.Books.Include(x => x.Authors).ToListAsync();
            var booksDTO = mapper.Map<IEnumerable<BookDTO>>(books);
            return booksDTO;

        }
        [HttpGet("{id:int}", Name = "GetBooks")]
        public async Task<ActionResult<BooksWithAuthorsDTO>> Get(int id)
        {
            var book = await context.Books
                .Include(x => x.Authors)
                    .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);//cuando incluyo la entidad Author , obtengo authorBoook puedo sacar Book para los datos relacionados Muchos a Muchos
            //var book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);//incluye la entidad autor
            if (book == null)
            {
                return NotFound();
            }
            var bookDTO = mapper.Map<BooksWithAuthorsDTO>(book);
            return Ok(bookDTO);
        }
        [HttpPost]
        public async Task<ActionResult> Post(BookCreateDTO bookCreateDTO)
        {
            if (bookCreateDTO.AuthorsId is null || bookCreateDTO.AuthorsId.Count==0)//validamos que no se venga un libro sin autores
            {
                ModelState.AddModelError(nameof(bookCreateDTO.AuthorsId), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            // Valido que los Id enviados dentro del array de AuthorsId, existan dentro de Authors, solo usando el Id
            var authorsExist = await context.Authors
                                            .Where(x => bookCreateDTO.AuthorsId.Contains(x.Id))
                                            .Select(x => x.Id).ToListAsync();
            if (authorsExist.Count != bookCreateDTO.AuthorsId.Count)//valido si lo que encontre tiene el mismo numero de coincidencias con lo que estoy ingresando, 
            {
                // si no es igual quiere decir que alguno de los id no existe en l abase de datos
                var authorsNotExist = bookCreateDTO.AuthorsId.Except(authorsExist);// tomo los id que no existen dentro de la base de datos
                var auhtorsNotExistString = string.Join(", ", authorsNotExist);//tomo los id y los concateno con comas para mostrar el mensaje
                var messageError = $"Los siguientes autores no existen:{auhtorsNotExistString}";//armo el mensaje de error
                ModelState.AddModelError(nameof(bookCreateDTO), messageError);
                return ValidationProblem();
            }






            var book = mapper.Map<Book>(bookCreateDTO);
            AsignateOrderAuthors(book);
            //var existAuthor = await context.Authors.AnyAsync(x => x.Id == book.AuthorId);
            //if (!existAuthor)
            //{
            //    //Creamos un error detallado desde el controlador
            //    ModelState.AddModelError(nameof(book.AuthorId), $"El autor de de ID {book.AuthorId} no existe");
            //    return ValidationProblem();
            //    //return  BadRequest($"El autor de de ID {book.AuthorId} no existe");
            //}

            context.Add(book);
            await context.SaveChangesAsync();
            var bookDTO = mapper.Map<BookDTO>(book);//sacamos el dto para el select final de la respuesta
            return CreatedAtRoute("GetBooks", new { id = book.Id }, bookDTO);
            //return Ok();
        }
        private void AsignateOrderAuthors(Book book) 
        {
            if (book.Authors is not null)
            {
                for (int i = 0; i < book.Authors.Count; i++)
                {
                    book.Authors[i].Order = i;// asignanos el order de los autores dentro del libro
                }
            }

        }
        [HttpPut("{id:int}")]// api/books/1
        public async Task<ActionResult> Put(int id, BookCreateDTO bookCreateDTO)
        {
            if (bookCreateDTO.AuthorsId is null || bookCreateDTO.AuthorsId.Count == 0)//validamos que no se venga un libro sin autores
            {
                ModelState.AddModelError(nameof(bookCreateDTO.AuthorsId), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            // Valido que los Id enviados dentro del array de AuthorsId, existan dentro de Authors, solo usando el Id
            var authorsExist = await context.Authors
                                            .Where(x => bookCreateDTO.AuthorsId.Contains(x.Id))
                                            .Select(x => x.Id).ToListAsync();
            if (authorsExist.Count != bookCreateDTO.AuthorsId.Count)//valido si lo que encontre tiene el mismo numero de coincidencias con lo que estoy ingresando, 
            {
                // si no es igual quiere decir que alguno de los id no existe en l abase de datos
                var authorsNotExist = bookCreateDTO.AuthorsId.Except(authorsExist);// tomo los id que no existen dentro de la base de datos
                var auhtorsNotExistString = string.Join(", ", authorsNotExist);//tomo los id y los concateno con comas para mostrar el mensaje
                var messageError = $"Los siguientes autores no existen:{auhtorsNotExistString}";//armo el mensaje de error
                ModelState.AddModelError(nameof(bookCreateDTO), messageError);
                return ValidationProblem();
            }
            //var book = mapper.Map<Book>(bookCreateDTO);
            //if (id != book.Id)
            //{
            //    return BadRequest("Los Ids deben coincidir");
            //}
            //var existAuthor = await context.Authors.SingleOrDefaultAsync(x => x.Id == book.AuthorId);
            //if (existAuthor == null)
            //{
            //    return BadRequest($"El autor de de ID {book.AuthorId} no existe");
            //}
            //context.Update(book);

            var bookDB = await context.Books
                               .Include(x => x.Authors)
                               .FirstOrDefaultAsync(x=> x.Id == id);//sacamos el id del libro
            if (bookDB == null)
            {
                return NotFound();
            }
            bookDB = mapper.Map(bookCreateDTO, bookDB);//asginamos los valores que vienen del DTO a la entidad consultada con mapper
            AsignateOrderAuthors(bookDB);//asginamos el orden de los autores dentro del libro
            await context.SaveChangesAsync();//guardamos los cambios
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleteFields = await context.Books.Where(x => x.Id == id).ExecuteDeleteAsync(); // eliminamos el registro
            if (deleteFields == 0)// si esto no devolvio registros
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
