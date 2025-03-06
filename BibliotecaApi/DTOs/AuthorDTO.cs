using BibliotecaApi.Entities;
using BibliotecaApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        public required string CompleteName { get; set; }
        //public List<BookDTO> Books { get; set; } = [];//sacamos el listado de libros y evitamos los valores nulos creando una lista vacia
    }
}
