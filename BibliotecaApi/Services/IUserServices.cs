using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Services
{
    public interface IUserServices
    {
        Task<IdentityUser?> GetUser();
    }
}