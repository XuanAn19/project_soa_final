using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Models
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<GenresModel> Genres { get; set; }

    }
}
