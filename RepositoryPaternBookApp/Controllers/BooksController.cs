using Microsoft.AspNetCore.Mvc;
using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Models.DomainModels;
using RepositoryPaternBookApp.Models.ViewModels;
using System.Reflection;


namespace RepositoryPaternBookApp.Controllers
{
	public class BooksController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public BooksController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 3)
		{
			var (books, count) = await _unitOfWork.BooksRelated.GetAllBooksWithAuthorsAndGenresAsync(pageNumber, pageSize);

			var booksViewModels = books.Select(b => new BookIndexViewModel
			{
				BookId = b.BookId,
				Title = b.Title,
				AuthorName = b.Author.Name,
				GenreNames = b.BookGenres.Select(bg => bg.Genre.Name).ToList(),
				IsAvailable = b.IsAvailable,
				IsBestSeller = b.IsBestSeller,
				IsNewRelease = b.IsNewRelease,
				BindingType = b.BindingType.ToString(),
			}).ToList();

			return View();
		}
		public async Task<IActionResult> Create()
		{
			var viewModel = new CreateBookViewModel
			{
				Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList(),
				Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList()
			};
			return View(viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateBookViewModel viewModel)
		{
			//Si la data entrée n'est pas valide il faut montrer qqch sinon crash
			if (!ModelState.IsValid)
			{
				viewModel.Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList();
				viewModel.Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList();
			}
			string imagePath = await SaveImageAsync(viewModel.Image);

			if (imagePath == null)
			{
				viewModel.Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList();
				viewModel.Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList();
				return View(viewModel);
			}

			var newBook = new Book
			{
				Title = viewModel.Book.Title,
				AuthorId = viewModel.SelectedAuthorId,
				IsAvailable = viewModel.Book.IsAvailable,
				IsBestSeller = viewModel.Book.IsBestSeller,
				IsNewRelease = viewModel.Book.IsNewRelease,
				BindingType = viewModel.Book.BindingType,
				ImagePath = imagePath,
			};
			await _unitOfWork.Books.AddAsync(newBook);
			await _unitOfWork.CompleteAsync();

			if (viewModel.SelectedGenres != null && viewModel.SelectedGenres.Any())
			{
				foreach (var genreId in viewModel.SelectedGenres)
				{
					var bookGenre = new BookGenre
					{
						BookId = newBook.BookId,
						GenreId = genreId,
					};
					await _unitOfWork.BooksGenres.AddAsync(bookGenre);
				}
				await _unitOfWork.CompleteAsync();
			}
			return RedirectToAction(nameof(Index));
		}

		
		private async Task<string> SaveImageAsync(IFormFile image)
		{
			if (image == null || image.Length == 0)
			{
				return null;
			}

			string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
			string filePath = Path.Combine(uploadFolder, uniqueFileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(fileStream);
			}

			return "/images/" + uniqueFileName;
		}
	}
}

