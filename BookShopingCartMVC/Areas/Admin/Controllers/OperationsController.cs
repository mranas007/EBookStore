using System.Data;
using Microsoft.AspNetCore.Mvc;
using BookShopingCartMVC.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookShopingCartMVC.Repository.IRepository;
using BookShopingCartMVC.Services.IServices;
using BookShopingCartMVC.Models.ViewModels;
using System.Text;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OperationsController(IUserOrderRepository _userOrderRepo, IPdfGenerator _pdfGenerator) : Controller
    {
        //private readonly IUserOrderRepository _userOrderRepo = userOrderRepository;
        //private readonly IPdfGenerator _pdfGenerator = pdfGenerator;

        //***** generate Order details PDF *****//
        [HttpGet("generate-order-pdf/{orderId}")]
        public async Task<IActionResult> GenerateOrderPdf(int orderId)
        {
            // 1. Get data from the database
            var orderDetails = await _userOrderRepo.GetOrderDetailsByIdAsync(orderId);

            if (orderDetails == null)
            {
                return NotFound();
            }
            //return Ok(orderDetails);
            // 2. Use the service to generate the PDF (QuestPDF)
            byte[] pdfBytes = _pdfGenerator.GeneratePdfForOrderList(orderDetails);

            // 5. Return the file
            return File(pdfBytes, "application/pdf", $"Order_{orderId}.pdf");
        }

        //***** list all user orders *****//
        public async Task<IActionResult> AllOrders(string orderStatus = "")
        {
            var orders = await _userOrderRepo.UserOrderAsync(true);
            var orderStatuses = await _userOrderRepo.GetOrderStatuses();

            // Apply filter if specified
            if (!string.IsNullOrEmpty(orderStatus))
            {
                orders = orders.Where(o => o.OrderStatusId.ToString() == orderStatus).ToList();
            }



            // Create dropdown list with "All Statuses" option
            var orderStatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All Statuses", Value = "", Selected = string.IsNullOrEmpty(orderStatus) }
            };

            // Add status options
            foreach (var status in orderStatuses)
            {
                orderStatusList.Add(new SelectListItem
                {
                    Text = status.StatusName,
                    Value = status.Id.ToString(),
                    Selected = status.Id.ToString() == orderStatus
                });
            }

            var viewModel = new Orders_StatusesViewModel
            {
                Orders = orders,
                OrderStatus = orderStatusList,
                Status = orderStatus
            };

            return View(viewModel);
        }

        //***** toggle payment for paid & unpaid *****//
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

        //***** update order view
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

        //***** update order status
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

        //***** delete user order
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                // Add delete logic here - you'll need to implement this in your repository
                //await _userOrderRepo.DeleteOrderAsync(orderId);
                //TempData["message"] = "Order deleted successfully";
                TempData["message"] = "Delete funationality is unavailable";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction(nameof(AllOrders));
        }
    }
}
