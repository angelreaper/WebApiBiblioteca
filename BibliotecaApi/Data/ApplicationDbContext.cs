using BibliotecaApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Data
{
    //public class ApplicationDBContext : DbContext
    public class ApplicationDBContext : IdentityDbContext<User> // usamos el sistemas de usuarios de netcore//indicamos que ahora la clase User va a representar mi usuario
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}
