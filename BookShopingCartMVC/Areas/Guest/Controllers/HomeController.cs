using System.Diagnostics;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signManager = signManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> BooksCollection()
        {
            var books = await _unitOfWork.Book.GetBooksAsync();
            return View(books);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
