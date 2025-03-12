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

        public SecurityController(IDataProtectionProvider protectionProvider)
        {
            protector = protectionProvider.CreateProtector("prueba");
            protectorLimited = protector.ToTimeLimitedDataProtector();
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

