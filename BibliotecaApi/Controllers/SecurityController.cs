using BibliotecaApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers
{
    [Route("api/Security")]
    [ApiController]
    public class SecurityController:ControllerBase
    {
        private IDataProtector protector;
        private ITimeLimitedDataProtector protectorLimited;
        private readonly IHashService hashService;

        public SecurityController(IDataProtectionProvider protectionProvider,IHashService hashService)
        {
            protector = protectionProvider.CreateProtector("prueba");
            protectorLimited = protector.ToTimeLimitedDataProtector();
            this.hashService = hashService;
        }

        [HttpGet("hash")]
        public ActionResult GetHash(string plainText)
        {
            var hash1 = hashService.Hash(plainText);
            var hash2 = hashService.Hash(plainText);
            var hash3 = hashService.HashEncrypt(plainText, hash2.Sal);//enviamos la sal del anterior hash para probar
            var result = new { plainText, hash1, hash2, hash3 };
            return Ok(result);

        }
        [HttpGet("encrypt-limit")]
        public ActionResult EncryptLimited(string plainText)
        {
            string ecnryptText = protectorLimited.Protect(plainText,lifetime:TimeSpan.FromSeconds(20));
            return Ok(new { ecnryptText });
        }
        [HttpGet("decrypt-limit")]
        public ActionResult DecryptLimited(string plainText)
        {
            string ecnryptText = protectorLimited.Unprotect(plainText);
            return Ok(new { ecnryptText });
        }

        [HttpGet("encrypt")]
        public ActionResult Encrypt(string plainText)
        { 
            string ecnryptText =protector.Protect(plainText);
            return Ok(new { ecnryptText });
        }
        [HttpGet("decrypt")]
        public ActionResult Decrypt(string plainText)
        {
            string ecnryptText = protector.Unprotect(plainText);
            return Ok(new { ecnryptText });
        }
    }
}

