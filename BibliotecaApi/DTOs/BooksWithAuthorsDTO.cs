namespace BibliotecaApi.DTOs
{
    public class BooksWithAuthorsDTO:BookDTO
    {
        //public int AuthorId { get; set; }//llave foranea
        //public required string AuthorName { get; set; }

        public List<AuthorDTO> Authors { get; set; } = [];//ahora debemos sacar la lista de autores relacionado al libro
    }
}
