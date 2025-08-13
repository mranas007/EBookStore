using System.Diagnostics;
using System.Text.Json;
using BookShopingCartMVC.Repository.IRepository;
using BookShopingCartMVC.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signManager = signManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            if (_signManager.IsSignedIn(User) && User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            else if (_signManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Book", new { area = "Admin" });
            }
            return View();
        }

        public async Task<IActionResult> BooksCollection(string sterm = "", int genreId = 0, int pageNumber = 1, int pageSize = 20)
        {
            PaginationParams paginationParams = new PaginationParams { PageNumber = pageNumber, PageSize = pageSize };
            IEnumerable<Book> books = await _unitOfWork.Book.GetAllAsync(sterm, genreId, paginationParams);
            IEnumerable<Genre> genres = await _unitOfWork.Genre.GetAllAsync();
            BookDisplayViewModel bookModel = new BookDisplayViewModel
            {
                Books = books,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalBooks = await _unitOfWork.Book.GetCountAsync(sterm, genreId)
            };
            return View(bookModel);

            //var options = new JsonSerializerOptions
            //{
            //    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            //    MaxDepth = 64
            //};
            //string jsonString = JsonSerializer.Serialize(books, options);
            //return Ok(jsonString);
        }

        public async Task<IActionResult> BookDetails(int id)
        {
            try
            {
                var book = await _unitOfWork.Book.GetByIdAsync(id);
                return View(book);
            }
            catch (Exception ex)
            {
                var exception = new
                {
                    Message = ex.Message,
                    Success = false,
                    ExceptionType = ex.GetType().Name
                };
                return BadRequest(exception);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
