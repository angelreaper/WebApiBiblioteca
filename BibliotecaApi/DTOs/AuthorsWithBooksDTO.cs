namespace BibliotecaApi.DTOs
{
    public class AuthorsWithBooksDTO:AuthorDTO
    {
        public List<BookDTO> Books { get; set; } = [];//sacamos el listado de libros y evitamos los valores nulos creando una lista vacia
    }
}
