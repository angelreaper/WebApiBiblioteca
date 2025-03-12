using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Services
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor contextAccessor;

        public UserServices(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
        {
            this.userManager = userManager;
            this.contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Método que devuelve el email del usuario desde su claim para poder consultar la base de datos
        /// desde la petición http
        /// </summary>
        /// <returns></returns>
        public async Task<User?> GetUser()// con "?" indicamos que esto puede ser nulo
        {
            var emailClaim = contextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault();// esto es por el nombre del claim que tengo en CreateToken() del controlador User lo llamo por la clave del claim
            if (emailClaim is null)
            {
                return null;
            }
            var email = emailClaim.Value;//sacamos el email con el cual se estan logueando de los claims
            return await userManager.FindByEmailAsync(email);

        }
    }
}
