using Microsoft.AspNetCore.Mvc;
using RepositoryPaternBookApp.Interfaces;

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
    }
}
