using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class BookCreateDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Title { get; set; }
        public List<int> AuthorsId { get; set; } = [];//llave foranea

    }
}
