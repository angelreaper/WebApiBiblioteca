using AutoMapper;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BibliotecaApi.Controllers
{
    [ApiController]
    [Route("api/authors-collection")]
    //[Authorize]//protegemos a nivel de controllador el acceso a sus endpoints
    [Authorize(Policy = "isadmin")]//protegemos a nivel de controllador el acceso a sus endpoints--solo el admin puede usar estos metodos

    public class AuthorsCollectionController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public AuthorsCollectionController(ApplicationDBContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet("{ids}", Name = "GetAuthorsForIds")]// api/authors-collection/1,2,3
        public async Task<ActionResult> Get(string ids)
        { 
            var idsCollection = new List<int>();
            foreach (var id in ids.Split(","))
            {
                if (int.TryParse(id , out int idInt))
                {
                    idsCollection.Add(idInt);//metemos a la colección los id que voy a sacar del query string
                }
            }
            if (!idsCollection.Any())
            {
                ModelState.AddModelError(nameof(ids), "Ningún id fue encontrado");
                return ValidationProblem();
            }
            //consultamos los ids en la base de datos
            var authors = await context.Authors.Include(x => x.Books)
                                                    .ThenInclude(x => x.Book)
                                                .Where(x => idsCollection.Contains(x.Id))
                                                .ToListAsync();
            if (authors.Count() != idsCollection.Count())
            {

                return NotFound();
            }

            var authorsDTO = mapper.Map<List<AuthorsWithBooksDTO>>(authors);
            return Ok(authorsDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Post(IEnumerable<AuthorCreateDTO> authorsCreateDTO) {

            var authors = mapper.Map<IEnumerable<Author>>(authorsCreateDTO);//llenamos una colección de autores con el DTO
            context.AddRange(authors);//agregamos la colección pora que se ingrese a la base de datos
            await context.SaveChangesAsync();

            var authorsDTO = mapper.Map<IEnumerable<AuthorDTO>>(authors);//llenamos la respuesta
            var ids = authors.Select(x => x.Id).ToList();
            var idString = string.Join(",",ids);
            return CreatedAtRoute("GetAuthorsForIds", new { ids = idString }, authorsDTO);
            //return Ok();
        }

    }
}
