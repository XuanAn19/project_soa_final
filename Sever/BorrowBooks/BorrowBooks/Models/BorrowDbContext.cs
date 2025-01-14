using Microsoft.EntityFrameworkCore;

namespace BorrowBooks.Models
{
    public class BorrowDbContext : DbContext
    {
        public BorrowDbContext(DbContextOptions<BorrowDbContext> options) : base(options) { }
        public DbSet<BorrowModel> Borrows { get; set; }

    }
}
