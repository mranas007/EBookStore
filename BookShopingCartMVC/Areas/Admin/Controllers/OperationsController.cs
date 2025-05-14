using System.Data;
using Microsoft.AspNetCore.Mvc;
using BookShopingCartMVC.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BookShopingCartMVC.Repository;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OperationsController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;
        public OperationsController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepo = userOrderRepository;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderRepo.UserOrderAsync(true);

            //var options = new JsonSerializerOptions
            //{
            //    ReferenceHandler = ReferenceHandler.Preserve,
            //    WriteIndented = true
            //};
            //var json = JsonSerializer.Serialize(orders, options);
            //return Content(json, "application/json");

            return View(orders);
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderRepo.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction(nameof(AllOrders));
        }

        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _userOrderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return View(order);

            var orderStatusList = (await _userOrderRepo.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem
                {
                    Text = orderStatus.StatusName,
                    Value = orderStatus.Id.ToString(),
                    Selected = orderStatus.Id == order.OrderStatusId
                };
            });

            var data = new UpdateOrderStatusDto
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusDto data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    data.OrderStatusList = (await _userOrderRepo.GetOrderStatuses()).Select(orderStatus =>
                    {
                        return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = orderStatus.Id == data.OrderStatusId };
                    });

                    return View(data);
                }
                await _userOrderRepo.ChangeOrderStatus(data);
                TempData["message"] = "Updated successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }
    }
}
