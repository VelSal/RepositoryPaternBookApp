using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
        IRepository<Book> Books { get;  }
		IRepository<Author> Authors { get; }
		IRepository<Genre> Genres { get; }
		IRepository<BookGenre> BooksGenres { get; }
		IBookRepository BooksRelated {  get; }

		Task<int> CompleteAsync();
    }
}
