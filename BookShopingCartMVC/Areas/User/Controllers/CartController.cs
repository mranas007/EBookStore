using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public async Task<IActionResult> AddItem(int bookId, int quantity = 1, int redirect = 0)
        {
            if (bookId <= 0 || quantity <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid book ID or quantity." });
            }

            try
            {
                var cartItem = await _cartRepo.AddItemAsync(bookId, quantity);

                if (cartItem <= 0)
                {
                    return BadRequest(new { success = false, message = "Failed to add item to the cart." });
                }

                if (redirect == 0)
                {
                    return Ok(new { success = true, message = "Item added to the cart successfully.", data = cartItem });
                }
                TempData["success"] = "Cart is added successfully!";
                return RedirectToAction(nameof(GetUserCart));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while adding the item.", error = ex.Message });
            }
        }

        public async Task<IActionResult> RemoveItem(int bookId)
        {
            try
            {
                var result = await _cartRepo.RemoveItemAsync(bookId);
                if (result == 0)
                {
                    TempData["message"] = "Cart not Found!";
                }
                else
                {
                    TempData["success"] = "Cart is removed!";
                }
                return RedirectToAction(nameof(GetUserCart));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong! : " + ex.Message;
                return RedirectToAction(nameof(GetUserCart));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAllItem()
        {
            try
            {
                var result = await _cartRepo.RemoveAllItemAsync();
                if (result == 1)
                {
                    TempData["success"] = "All Cart are removed!";
                }
                else if (result == 0)
                {
                    TempData["message"] = "Cart aren't Found!";
                }
                return RedirectToAction(nameof(GetUserCart));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong! : " + ex.Message;
                return RedirectToAction(nameof(GetUserCart));
            }
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetAllCartAsync();
            if (cart == null)
                ViewData["Error"] = "sorry there is no cart";
            return View(cart);
        }

        // this is used for API method
        public async Task<IActionResult> GetCartItemCount()
        {
            var totalCart = await _cartRepo.GetCartItemCountAsync();
            return Ok(totalCart);
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartRepo.GetAllCartAsync();
            if (cart == null)
                return RedirectToAction(nameof(GetUserCart));

            double subAmount = 0;
            double shippingAmount = 10.00;
            foreach (var price in cart.CartDetails)
            {
                subAmount += price.UnitPrice;
            }
            ViewBag.SubTotal = subAmount;
            ViewBag.Shipping = shippingAmount;
            ViewBag.Total = subAmount + shippingAmount;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isCheckOut = await _cartRepo.DoCheckoutAsync(model);
            if (!isCheckOut)
                return RedirectToAction(nameof(OrderFailure));

            return RedirectToAction(nameof(OrderSuccess));
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }

    }
}
