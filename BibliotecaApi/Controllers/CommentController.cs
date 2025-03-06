using AutoMapper;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BibliotecaApi.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    [Authorize]//protegemos a nivel de controllador el acceso a sus endpoints
    public class CommentController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CommentController( ApplicationDBContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int booKId)
        {
            var existBook = await context.Books.AnyAsync(x => x.Id == booKId);

            if (!existBook)
            {
                return NotFound();
            }
             var comments = await context.Comments
                    .Include(x => x.User)//agregamos la propiedad de navegación el usuario
                    .Where(x => x.BookId == booKId)
                    .OrderByDescending(x => x.DateOfPublish)
                    .ToListAsync();
            return mapper.Map<List<CommentDTO>>(comments);
        }
        [HttpGet("{id}", Name = "GetComment")]//usamos id sin tipo de dato ya que es un GUID, ya que en si vamos a pasar un string en la URL
        public async Task<ActionResult<CommentDTO>> Get(Guid id)
        { 
            var comment = await context.Comments
                                    .Include(x=> x.User)//metemos la propiedad de navegación usuario
                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (comment is null)
            {
                return NotFound();
            }

            return mapper.Map<CommentDTO>(comment);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            var existBook = await context.Books.AnyAsync(x => x.Id == bookId);
            if (!existBook)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;
            comment.DateOfPublish = DateTime.UtcNow;
            context.Add(comment);
            await context.SaveChangesAsync();
            
            var commentDTO = mapper.Map<CommentDTO>(comment);  
            
            return CreatedAtRoute("GetComment", new { id = comment.Id, bookId }, commentDTO );


        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id,int bookId, JsonPatchDocument<CommentPatchDTO> commentDoc)
        {
            if (commentDoc is null)
            {
                return BadRequest();
            }

            var bookDB = await context.Books.AnyAsync(x => x.Id == bookId);

            if (!bookDB)
            {
                return NotFound();
            }
            var commentDB = await context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (commentDB is null)
            {
                return NotFound();
            }

            var commentPatchDTO = mapper.Map<CommentPatchDTO>(commentDB);

            commentDoc.ApplyTo(commentPatchDTO, ModelState);// le aplicamos los cambios

            var isValid = TryValidateModel(commentPatchDTO);

            if (!isValid)
            {
                return ValidationProblem();
            }

            mapper.Map(commentPatchDTO, commentDB);

            await context.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, int bookId)
        {

            var bookDB = await context.Books.AnyAsync(x => x.Id == bookId);

            if (!bookDB)
            {
                return NotFound();
            }

            var deleteFields = await context.Comments.Where(x => x.Id == id).ExecuteDeleteAsync(); // eliminamos el registro
            if (deleteFields == 0)// si esto no devolvio registros
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
