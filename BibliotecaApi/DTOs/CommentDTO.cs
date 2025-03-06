using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public required string Body { get; set; }
        public DateTime DateOfPublish { get; set; }
    }
}
