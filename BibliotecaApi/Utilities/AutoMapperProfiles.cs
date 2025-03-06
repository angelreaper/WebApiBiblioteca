using AutoMapper;
using BibliotecaApi.DTOs;
using BibliotecaApi.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;

namespace BibliotecaApi.Utilities
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            //Author
            CreateMap<Author, AuthorDTO>()
                //.ForMember(dto => dto.CompleteName, config => config.MapFrom(author => $"{author.Name} {author.LastName}"));
                .ForMember(dto => dto.CompleteName, config => config.MapFrom(author => MapNameAndLastName(author)));
            CreateMap<Author, AuthorsWithBooksDTO>()
                .ForMember(dto => dto.CompleteName, config => config.MapFrom(author => MapNameAndLastName(author)));
            CreateMap<AuthorCreateDTO, Author>();
            CreateMap<Author, AuthorPatchDTO>().ReverseMap();// ReversMap() para que tenga correspondencia del uno al otro
            //Book
            CreateMap<Book, BookDTO>();

            //CreateMap<Book, BooksWithAuthorsDTO>()
            //    //.ForMember(dto => dto.AuthorName, config => config.MapFrom(book => $"{book.Author!.Name} {book.Author.LastName}"));
            //    .ForMember(dto => dto.AuthorName, config => config.MapFrom(book => MapNameAndLastName(book.Author!)));
            CreateMap<Book, BooksWithAuthorsDTO>();
            CreateMap<BookCreateDTO, Book>().ForMember(ent => ent.Authors, config => config.MapFrom(dto => dto.AuthorsId.Select(id => new AuthorBook { AuthorId = id })));//hago el mapeo de los ID que estan dentro de dto hacia el campo Authors de la entidad Book
            //Comment
            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<CommentPatchDTO, Comment>().ReverseMap();

            //Mapeos de AuthorBooks con BookDTO este es para sacar el listado de libros que le corresponden a un autor
            //Esto se saca para el campo Books de la entidad Author
            //Para mostrar el BookDTO y cargar el campo Books de AuthorWithBooks
            CreateMap<AuthorBook, BookDTO>()
                .ForMember(dto=> dto.Id,config => config.MapFrom(ent => ent.BookId))
                .ForMember(dto=> dto.Title,config => config.MapFrom(ent => ent.Book!.Title));

            //Mapeos de AuthorBooks con AuthorDTO este es para sacar el listado de authores que le corresponden a un libro
            //Esto se saca para el campo Authors de la entidad Book
            //Para mostrar el AuthroDTO y cargar el campo Ahthors de BooksWithAuthors
            CreateMap<AuthorBook, AuthorDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AuthorId))
                .ForMember(dto => dto.CompleteName, config => config.MapFrom(ent => MapNameAndLastName(ent.Author! )));//enviamos el author para sacar el nombre completo


            // para crear un autor con sus libros
            //debemos mapear los campos List<BookCreateDTO> Books del DTO hacia el campo List<AuthorBook> Books de la entidad Author
            CreateMap<BookCreateDTO,AuthorBook>()
                .ForMember(ent => ent.Book,config => config.MapFrom(dto => new Book { Title = dto.Title } ));// para crear el libro


        }
        private string MapNameAndLastName(Author author) => $"{author.Name} {author.LastName}";
    }
}
