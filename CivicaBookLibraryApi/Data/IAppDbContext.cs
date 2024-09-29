using CivicaBookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaBookLibraryApi.Data
{
    public interface IAppDbContext : IDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookIssue> BookIssues { get; set; }


    }
}
