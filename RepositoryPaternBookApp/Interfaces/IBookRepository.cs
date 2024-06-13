using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Interfaces
{
	public interface IBookRepository:IRepository<Book>
	{
		Task<(IEnumerable<Book> Books, int count)> GetAllBooksWithAuthorsAndGenresAsync(int pageNumber, int pageSize);
		Task<Book> GetBookWithGenresAsync(int id);
		Task<Book> GetBookWithGenresAndAuthorsAsync(int id);
	}
}
