using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs
{
    public class CredentialUserDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public string ?Password { get; set; }
    }
}
