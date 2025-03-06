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
    }
}
