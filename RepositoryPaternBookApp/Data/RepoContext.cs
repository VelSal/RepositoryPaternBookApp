using Microsoft.EntityFrameworkCore;
using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Data
{
	public class RepoContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<Genre> Genres { get; set; } 
        public RepoContext(DbContextOptions<RepoContext> options) : base(options)
		{
        }
    }
}
