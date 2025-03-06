using BibliotecaApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [Required (ErrorMessage ="El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength (150, ErrorMessage ="El campo {0} debe tener {1} caracteres o menos")]
        [FirstMayusAttribute]
        public  required string Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [FirstMayusAttribute]
        public required string LastName { get; set; }
        public string? Identification { get; set; }
        //public List<Book> Books { get; set; } = new List<Book>();//sacamos el listado de libros y evitamos los valores nulos creando una lista vacia
        public List<AuthorBook> Books { get; set; } = [];//sacamos la relación con la tabla authorbook
    }
}
