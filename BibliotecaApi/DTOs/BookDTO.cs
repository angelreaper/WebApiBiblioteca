using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }

       // public int AuthorId { get; set; }//llave foranea

        
    }
}
