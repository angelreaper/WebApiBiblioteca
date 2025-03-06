using BibliotecaApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Title { get; set; }
        //public int AuthorId { get; set; }//llave foranea
        //public Author? Author { get; set; }// relación hacia la tabla autor
        public List<AuthorBook> Authors { get; set; } = [];//aquí sacamos la lista de autores
        public List<Comment> Comments { get; set; } = [];// relación hacia comentarios
    }
}
