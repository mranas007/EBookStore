using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookShopingCartMVC.Repository.IRepository;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _userRepo;

        public UserController(IApplicationUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepo.GetAllAsync();
            return View(users);
        }
    }
}