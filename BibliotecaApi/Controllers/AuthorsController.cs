using AutoMapper;
using Azure;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace BibliotecaApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    //[Authorize]//protegemos a nivel de controllador el acceso a sus endpoints
    [Authorize(Policy = "isadmin")]//protegemos a nivel de controllador el acceso a sus endpoints--solo el admin puede usar estos metodos
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDBContext context,ILogger<AuthorsController>logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        //[HttpGet("/list-of-authors")]// api/list-authors--> creamos una ruta personalizada para que de respuesta desde esa url
        [HttpGet]//api/authors también desde aquí puede dar la misma respuesta
        [AllowAnonymous]
        public async Task<IEnumerable<AuthorDTO>> Get()
        {
            //logger.LogTrace("Obteniendo Listado Trace");
            //logger.LogDebug("Obteniendo Listado Debug");
            //logger.LogInformation("Obteniendo Listado Information");
            //logger.LogWarning("Obteniendo Listado Warning");
            //logger.LogCritical("Obteniendo Listado Critical");
            //return await context.Authors.Include(x=> x.Books).ToListAsync();
            var author = await context.Authors.Include(x => x.Books).ToListAsync();
            var authorDTO = mapper.Map<IEnumerable<AuthorDTO>>(author);
            return authorDTO;
        }

        //[HttpGet("first")]//api/authors/first --> esto quita la ambiguedad de respuesta de get dando una ruta más 
        //public async Task<Author> GetPrimero()
        //{
        //    return await context.Authors.Include(x => x.Books).FirstAsync();
        //}
        [HttpGet("{id:int}",Name ="GetAuthors")]
        [AllowAnonymous]
        [EndpointSummary("Obtiene autor por Id")]
        [EndpointDescription("Obtiene un autor por su id , inclyendo sus libros,si no existe devuelve un 404.")]
        [ProducesResponseType<AuthorsWithBooksDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ActionResult>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorsWithBooksDTO>> Get([Description("El id del autor")]int id)
        { 
            var author = await context.Authors
                        .Include(x => x.Books)
                            .ThenInclude(x=> x.Book)//cuando incluyo la entidad Books , obtengo authorBoook puedo sacar Book para los datos relacionados Muchos a Muchos
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }
            var authorDTO= mapper.Map<AuthorsWithBooksDTO>(author);   //mapeamos la respuesta desde Author y devolvemos el DTO
            return Ok(authorDTO);
        }

        //[HttpGet("{parametro1}/{parametro2?}")]// api/authors/hola/mundo --> esto muestra un Json con los datos que ese enviaron en la ruta
        //public IActionResult Get(string parametro1,string parametro2="valor por defecto")
        //{
            
        //    return Ok(new {parametro1, parametro2 });
        //}
        [HttpPost]
        public async Task<ActionResult> Post(AuthorCreateDTO authorCreateDTO)
        { 
            var author = mapper.Map<Author>(authorCreateDTO);//mapeamos desde authorDTO y ingresamos la entidad Author creada
            context.Add(author);
            await context.SaveChangesAsync();
            var authorDTO = mapper.Map<AuthorDTO>(author);//sacamos el dto para el autor
            
            return CreatedAtRoute("GetAuthors", new { id = author.Id }, authorDTO);
            //return Ok();
        }
        [HttpPut("{id:int}")]// api/authors/1
        public async Task<ActionResult> Put(int id, AuthorCreateDTO authorDTO) {
            var author = mapper.Map<Author>(authorDTO);
            if (id != author.Id)
            {
                return BadRequest("Los Ids deben coincidir");
            }
            context.Update(author);
            await context.SaveChangesAsync();  
            return NoContent();    
        }
        [HttpPatch("{id:int}")]// api/authors/1
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AuthorPatchDTO> authorDoc)
        {
            if (authorDoc is null)
            {
                return BadRequest();
            }

            var authorDB = await context.Authors.FirstOrDefaultAsync(x => x.Id == id);

            if (authorDB is null)
            {
                return NotFound();
            }

            var authorPatchDTO = mapper.Map<AuthorPatchDTO>(authorDB);

            authorDoc.ApplyTo(authorPatchDTO, ModelState);// le aplicamos los cambios

            var isValid = TryValidateModel(authorPatchDTO);

            if (!isValid)
            {
                return ValidationProblem();
            }

            mapper.Map(authorPatchDTO, authorDB);

            await context.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        { 
            var registrosBorrados = await context.Authors.Where(x=> x.Id == id).ExecuteDeleteAsync(); // eliminamos el registro
            if (registrosBorrados == 0)// si esto no devolvio registros
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
