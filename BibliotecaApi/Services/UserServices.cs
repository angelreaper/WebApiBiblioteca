using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Services
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor contextAccessor;

        public UserServices(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            this.userManager = userManager;
            this.contextAccessor = contextAccessor;
        }


        public async Task<IdentityUser?> GetUser()// con "?" indicamos que esto puede ser nulo
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
