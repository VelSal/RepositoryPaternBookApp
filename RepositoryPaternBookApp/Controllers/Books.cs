using Microsoft.AspNetCore.Mvc;
using RepositoryPaternBookApp.Interfaces;
using RepositoryPaternBookApp.Models.DomainModels;
using RepositoryPaternBookApp.Models.ViewModels;
using System.Reflection;


namespace RepositoryPaternBookApp.Controllers
{
	public class Books : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public Books(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
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
				await InvalidData(viewModel);
			}
			string imagePath = await SaveImageAsync(viewModel.Image);

			if (imagePath == null)
			{
				await InvalidData(viewModel);
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

		private async Task InvalidData(CreateBookViewModel viewModel)
		{
			viewModel.Authors = (await _unitOfWork.Authors.GetAllAsync()).ToList();
			viewModel.Genres = (await _unitOfWork.Genres.GetAllAsync()).ToList();
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

