using BibliotecaApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class AuthorPatchDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [FirstMayusAttribute]
        public required string Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]// este es el required del framework, el required del campo es diferente y pertenece al lenguaje como tal
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [FirstMayusAttribute]
        public required string LastName { get; set; }
        public string? Identification { get; set; }
    }
}
