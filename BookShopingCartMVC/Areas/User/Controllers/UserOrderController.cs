using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrder;
        public UserOrderController(IUserOrderRepository userOrder)
        {
            _userOrder = userOrder;
        }

        public async Task<IActionResult> UserOrders()
        {
            var userOrder = await _userOrder.UserOrderAsync();
            if (userOrder == null)
                return View("Error", "User Order is null!");

            return View(userOrder);
        }
    }
}
