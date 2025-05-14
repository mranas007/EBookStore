using BookShopingCartMVC.Repository.IRepository;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace BookShopingCartMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepo;
        private readonly ILikeRepository _likeRepo;

        public HomeController(ILogger<HomeController> logger, 
            IUnitOfWork unitOfWork, 
            ICartRepository cartRepository,
            ILikeRepository likeRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartRepo = cartRepository;
            _likeRepo = likeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var totalCart = await _cartRepo.GetCartItemCountAsync();
            var totalLike = await _likeRepo.GetLikeCountAsync();
            ViewBag.totalCart = totalCart;
            ViewBag.favourite = totalLike;

            return View();
        }

        public async Task<IActionResult> Books(string sterm = "", int genreId = 0)
        {
            IEnumerable<Book> books = await _unitOfWork.Book.GetAllAsync(sterm, genreId);
            IEnumerable<Genre> genres = await _unitOfWork.Genre.GetAllAsync();
            BookDisplayViewModel bookModel = new BookDisplayViewModel
            {
                Books = books,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId
            };
            return View(bookModel);
        }
        public async Task<IActionResult> BookDetails(int id)
        {
            try
            {
                var book = await _unitOfWork.Book.GetByIdAsync(id);
                //var options = new JsonSerializerOptions
                //{
                //    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                //};
                //return Json(book, options);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
