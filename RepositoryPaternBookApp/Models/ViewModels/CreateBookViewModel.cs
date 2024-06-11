using RepositoryPaternBookApp.Models.DomainModels;

namespace RepositoryPaternBookApp.Models.ViewModels
{
	public class CreateBookViewModel
	{
        public Book Book { get; set; }
        public int SelectedAuthorId { get; set; }
        public List<int> SelectedGenres { get; set; } = new List<int>();
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public IFormFile Image { get; set; }
        public string? ImagePath { get; set; } = "/images/Default.png";
    }
}
