using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        [Required]
        public required string Body { get; set; }
        public DateTime DateOfPublish { get; set; }
        public int BookId {  get; set; }// llave foranea
        public Book? Book { get; set; }// propiedad de navegación para el libro
        // relación ahcia usuarios
        public required string UserId { get; set; }//llave foranea
        public User? User { get; set; }// relación para el usuario
    }
}
