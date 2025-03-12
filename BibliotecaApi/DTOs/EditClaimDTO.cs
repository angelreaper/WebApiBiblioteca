using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class EditClaimDTO
    {
        [EmailAddress]
        [Required]
        public required string Email { get; set; }
    }
}
