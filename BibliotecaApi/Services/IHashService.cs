using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services
{
    public interface IHashService
    {
        ResultHashDTO Hash(string input);
        ResultHashDTO HashEncrypt(string input, byte[] sal);
    }
}