using BibliotecaApi.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BibliotecaApi.Services
{
    public class HashService : IHashService
    {
        public ResultHashDTO Hash(string input)
        {
            var sal = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(sal);//llenamos el array con el dato random
            }
            return HashEncrypt(input, sal);
        }

        public ResultHashDTO HashEncrypt(string input, byte[] sal)
        {
            string hased = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: input,
                     salt: sal,
                     prf: KeyDerivationPrf.HMACSHA1,
                     iterationCount: 10_000,
                     numBytesRequested: 256 / 8
                ));

            return new ResultHashDTO { Hash = hased, Sal = sal };
        }
    }
}
