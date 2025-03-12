using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Entities
{
    public class User:IdentityUser
    {
        public DateTime DateOfBirth { get; set; }// creamos el campo
    }
}
