using AutoMapper;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    //[Authorize]
    public class UserController:ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<User> signInManager;
        private readonly IUserServices userServices;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public UserController( UserManager<User> userManager,IConfiguration configuration,SignInManager<User> signInManager, IUserServices userServices, ApplicationDBContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userServices = userServices;
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        [Authorize(Policy = "isadmin")]//solo los adminstradores pueden ver esto
        public async Task<IEnumerable<UserDTO>> Get()
        {
            var users = await context.Users.ToListAsync();
            var usersDTO = mapper.Map<IEnumerable<UserDTO>>(users);
            return usersDTO;

        }



        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseAutenticateUserDTO>> Register(CredentialUserDTO credentialUserDTO) {

            var user = new User
            {
                UserName = credentialUserDTO.Email,
                Email = credentialUserDTO.Email,
            };
            var result = await userManager.CreateAsync(user,credentialUserDTO.Password!);

            if (result.Succeeded)// si se hizo el registro
            {
                //creamos el token de autenticación
                var responseAutentication = await CreateToken(credentialUserDTO);
                return responseAutentication;
            }
            else 
            {
                foreach (var error in result.Errors)// iteramos en los errores que surgan
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem();
            }

        
        }

        [HttpPost("login")]
        //[AllowAnonymous]
        public async Task<ActionResult<ResponseAutenticateUserDTO>> Login(CredentialUserDTO credentialUserDTO)
        { 
            var user = await userManager.FindByEmailAsync(credentialUserDTO.Email);// validamos si existe el usuario

            if (user == null)
            { 
                return ReturnIncorrectLogin();
            }

            var result= await signInManager.CheckPasswordSignInAsync(user, credentialUserDTO.Password!,lockoutOnFailure:false);// con lockoutOnFailure:false indicamos que no se bloquee el usuario por inicios de sesion incorrectos
            if (result.Succeeded)// si es correcto el login
            {
                return await CreateToken(credentialUserDTO);
            }
            else
            {
                return ReturnIncorrectLogin();
            }

        }

        [HttpPost("set-admin")]
        [Authorize(Policy ="isadmin")]
        public async Task<ActionResult>SetAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);
            if (user is null) 
            { 
                return NotFound();
            }
            await userManager.AddClaimAsync(user, new Claim("isadmin", "true"));// nombre del claim y su valor}
            return NoContent();
        }

        [HttpPost("remove-admin")]
        [Authorize(Policy = "isadmin")]
        public async Task<ActionResult> RemoveAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);
            if (user is null)
            {
                return NotFound();
            }
            await userManager.RemoveClaimAsync(user, new Claim("isadmin", "true"));// nombre del claim y su valor}
            return NoContent();
        }

        private ActionResult ReturnIncorrectLogin()
        {

            ModelState.AddModelError(string.Empty, "Login Incorrecto");// dejamos esto para no dar información inecesaria hacia afuera por cuestiones de seguridad
            return ValidationProblem();


        }
        [HttpGet("renovate-token")]
        [Authorize]// solo los que tengan el token pueden ver el método
        public async Task<ActionResult<ResponseAutenticateUserDTO>> RenovateToken()
        {
            var user = await userServices.GetUser();
            if (user is null)
            {
                return NotFound();
            }
            var credentialUserDTO = new CredentialUserDTO { Email = user.Email! };
            var responsAutentication = await CreateToken(credentialUserDTO);
            return responsAutentication;

        }
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Put(UpdateUserDTO updateUserDTO)
        {
            var user = await userServices.GetUser();
            if (user is null)
            {
                return NotFound();
            }

            user.DateOfBirth = updateUserDTO.DateOfBirth;
            await userManager.UpdateAsync(user);

            return NoContent();


        }


        //método para creación del token
        private async Task<ResponseAutenticateUserDTO> CreateToken(CredentialUserDTO credentialUserDTO)
        {
            //creamos los claims
            var claims = new List<Claim>
                        { 
                            // llave , valor
                            new Claim("email",credentialUserDTO.Email),
                            new Claim("lo que sea","cualquier cosa")
                        };
            var user = await userManager.FindByEmailAsync(credentialUserDTO.Email);// buscamos el usuario por email
            var claimsDB = await userManager.GetClaimsAsync(user!);//sacamos el claim
            
            claims.AddRange(claimsDB);// agregamos los claims de la base de datos  a los claims
            // creación del token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]!));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);// usamos el algoritmo de HmacSha256 para la firma

            var expiration = DateTime.UtcNow.AddYears(1);// lo dejamos en 1 año por cuestiones de prueba

            // configuramos el token
            var tokenOfSecurity = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOfSecurity);// creamos el token

            return new ResponseAutenticateUserDTO { Token = token , Expiration = expiration};

        }


    }
}
