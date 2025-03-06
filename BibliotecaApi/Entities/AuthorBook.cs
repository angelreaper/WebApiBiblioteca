using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Entities
{
    [PrimaryKey(nameof(AuthorId),nameof(BookId))]// indicamos como se compone la llave primaria
    public class AuthorBook
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public int Order { get; set; }
        public Author? Author { get; set; }// relación hacia la tabla author
        public Book? Book { get; set; }// relación hacia la tabla book

    }
}
