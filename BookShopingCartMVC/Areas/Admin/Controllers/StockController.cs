using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepository)
        {
            _stockRepo = stockRepository;
        }


        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _stockRepo.GetStcoks(sTerm);
            return View(stocks);
        }
        public async Task<IActionResult> ManageStock(int bookId)
        {
            var existingStock = await _stockRepo.GetStcokById(bookId);
            var stock = new StockDto
            {
                BookId = bookId,
                Quantity = existingStock != null ? existingStock.Quantity : 0,
            };

            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManageStock(StockDto stock)
        {
            if (!ModelState.IsValid)
            {
                return View(stock);
            }
            try
            {
                await _stockRepo.ManageTask(stock);
                TempData["success"] = "Stock is Updated Successfully.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something Went Wrong!";
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
