using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		public IRepository<Book> Books => throw new NotImplementedException();

		public IRepository<Author> Authors => throw new NotImplementedException();

		public IRepository<Genre> Genres => throw new NotImplementedException();

		public IRepository<BookGenre> BooksGenres => throw new NotImplementedException();

		public Task<int> CompleteAsync()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
