namespace BibliotecaApi.DTOs
{
    public class ResponseAutenticateUserDTO
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }

    }
}
