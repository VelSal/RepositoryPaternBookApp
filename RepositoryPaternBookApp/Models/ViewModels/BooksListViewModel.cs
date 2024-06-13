using RepositoryPaternBookApp.Helper;

namespace RepositoryPaternBookApp.Models.ViewModels
{
	public class BooksListViewModel
	{
        public PaginatedList<BookIndexViewModel> Books { get; set; }
        public int TotalBooks { get; set; }
        public int PageSize { get; set; }
    }
}
