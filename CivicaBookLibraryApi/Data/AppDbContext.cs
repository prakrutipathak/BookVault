using CivicaBookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CivicaBookLibraryApi.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext : DbContext,IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<Book> Books{ get; set; }
        public DbSet<BookIssue> BookIssues {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.SecurityQuestion)
                .WithMany(c => c.Users)
                .HasForeignKey(p => p.PasswordHint)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }


        public EntityState GetEntryState<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).State;
        }

        public void SetEntryState<TEntity>(TEntity entity, EntityState entityState) where TEntity : class
        {
            Entry(entity).State = entityState;
        }
    }
}
