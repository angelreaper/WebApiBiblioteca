namespace BibliotecaApi.DTOs
{
    public class ResultHashDTO
    {
        public required string Hash { get; set; }
        public required byte[] Sal { get; set; }
    }
}
