using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Services
{
    public interface IUserServices
    {
        Task<User?> GetUser();
    }
}