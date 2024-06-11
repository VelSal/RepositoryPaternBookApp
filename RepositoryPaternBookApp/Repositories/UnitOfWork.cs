using RepositoryPaternBookApp.Data;
using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly RepoContext _context;

        public UnitOfWork(RepoContext context)
        {
			_context = context;
			Books = new Repository<Book>(_context);
			Authors = new Repository<Author>(_context);
			Genres = new Repository<Genre>(_context);
			BooksGenres = new Repository<BookGenre>(_context);
			BooksRelated = new BookRepository(_context);
        }

		public IRepository<Book> Books { get;  }
		public IRepository<Author> Authors { get; }
		public IRepository<Genre> Genres { get; }
		public IRepository<BookGenre> BooksGenres { get; }
		public IBookRepository BooksRelated { get; }

		public Task<int> CompleteAsync()
		{
			return _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
