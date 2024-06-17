using Microsoft.AspNetCore.Mvc;
using RepositoryPaternBookApp.Helper;
using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Models.DomainModels;
using RepositoryPaternBookApp.Models.ViewModels;

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
			if (Request.Cookies["PageSize"] != null && int.TryParse(Request.Cookies["PageSize"], out var storedPageSize))
			{
				pageSize = storedPageSize;
			}

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

			var paginatedBooks = new PaginatedList<BookIndexViewModel>(booksViewModels, count, pageNumber, pageSize);
			var viewModel = new BooksListViewModel
			{
				Books = paginatedBooks,
				TotalBooks = count,
			};

			ViewBag.PageSize = pageSize;
			return View(viewModel);
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
				return View(viewModel);
			}

			string? imagePath = viewModel.Image != null && viewModel.Image.Length > 0
					? await SaveImageAsync(viewModel.Image)
					: "/images/Default.png";

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

		public async Task<IActionResult> Edit(int id)
		{
			var book = await _unitOfWork.BooksRelated.GetBookWithGenresAsync(id);
			if (book == null)
			{
				return NotFound();
			}

			var bookViewModel = new EditBookViewModel
			{
				BookId = book.BookId,
				Title = book.Title,
				AuthorId = book.AuthorId,
				SelectedGenres = book.BookGenres.Select(bg => bg.GenreId).ToList(),
				IsAvailable = book.IsAvailable,
				IsNewRelease = book.IsNewRelease,
				IsBestSeller = book.IsBestSeller,
				BindingType = book.BindingType,
				ImagePath = book.ImagePath,
				Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList(),
				Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList(),
			};
			return View(bookViewModel);
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Edit(int id, EditBookViewModel viewModel)
		{
			if (id != viewModel.BookId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var book = await _unitOfWork.BooksRelated.GetBookWithGenresAsync(id);

				if (book == null) return NotFound();

				string? imagePath = book.ImagePath;
				if (viewModel.Image != null && viewModel.Image.Length > 0)
				{
					imagePath = await SaveImageAsync(viewModel.Image);
				}

				//update book
				book.Title = viewModel.Title;
				book.AuthorId = viewModel.AuthorId;
				book.IsAvailable = viewModel.IsAvailable;
				book.IsBestSeller = viewModel.IsBestSeller;
				book.IsNewRelease = viewModel.IsNewRelease;
				book.BindingType = viewModel.BindingType;
				book.ImagePath = imagePath;

				//update genres
				book.BookGenres.Clear();
				if (viewModel.SelectedGenres != null)
				{
					foreach (var genreId in viewModel.SelectedGenres)
					{
						book.BookGenres.Add(new BookGenre
						{
							BookId = book.BookId,
							GenreId = genreId
						});
					}
				}
				await _unitOfWork.Books.UpdateAsync(book);
				await _unitOfWork.CompleteAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				viewModel.Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList();
				viewModel.Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList();
				return View(viewModel);

			}
		}

		public async Task<IActionResult> Details(int id)
		{
			var book = await _unitOfWork.BooksRelated.GetBookWithGenresAndAuthorsAsync(id);
			if (book == null) return NotFound();

			var viewModel = new BookDetailsViewModel
			{
				BookId = book.BookId,
				Title = book.Title,
				AuthorName = book.Author.Name,
				GenreNames = book.BookGenres.Select(bg => bg.Genre.Name).ToList(),
				IsAvailable = book.IsAvailable,
				IsBestSeller = book.IsBestSeller,
				IsNewRelease = book.IsNewRelease,
				BindingType = book.BindingType.ToString(),
				ImagePath = book.ImagePath
			};

			return View(viewModel);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var book = await _unitOfWork.BooksRelated.GetBookWithGenresAndAuthorsAsync(id);
			if (book == null)
			{
				return NotFound();
			}
			var viewModel = new BookDetailsViewModel
			{
				BookId = book.BookId,
				Title = book.Title,
				AuthorName = book.Author.Name,
				GenreNames = book.BookGenres.Select(bg => bg.Genre.Name).ToList(),
				IsAvailable = book.IsAvailable,
				IsBestSeller = book.IsBestSeller,
				IsNewRelease = book.IsNewRelease,
				BindingType = book.BindingType.ToString(),
				ImagePath = book.ImagePath
			};

			return View(viewModel);
		}

		[HttpPost, ActionName(nameof(Delete)), AutoValidateAntiforgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _unitOfWork.Books.DeleteAsync(id);
			//TODO
			//await DeleteImage()
			await _unitOfWork.CompleteAsync();
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SetPageSize(int pageSize)
		{
			if (pageSize < 3 || pageSize > 20)
			{
				ModelState.AddModelError("PageSize", "Page size must be between 3 and 20");
				ViewBag.PageSize = pageSize;
				return View(nameof(Index), new BooksListViewModel { Books = new PaginatedList<BookIndexViewModel>(new List<BookIndexViewModel>(), 0, 1, pageSize) });
			}

			CookieOptions options = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
			};
			Response.Cookies.Append("PageSize", pageSize.ToString(), options);
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
		private void DeleteImage(string imagePath)
		{
			if(!string.IsNullOrEmpty(imagePath))
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
			}
		}
	}
}

