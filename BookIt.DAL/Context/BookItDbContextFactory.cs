using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookIt.DAL.Context
{
    public class BookItDbContextFactory : IDesignTimeDbContextFactory<BookItDbContext>
    {
        public BookItDbContext CreateDbContext(string[] args)
        {

            var options = new DbContextOptionsBuilder<BookItDbContext>();
            options.UseSqlServer("Server=.;Database=BookItDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");

            return new BookItDbContext(options.Options);
        }
    }
}
