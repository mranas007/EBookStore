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
    public class OperationsController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;
        private readonly IPdfGenerator _pdfGenerator;
        public OperationsController(IUserOrderRepository userOrderRepository, IPdfGenerator pdfGenerator)
        {
            _userOrderRepo = userOrderRepository;
            _pdfGenerator = pdfGenerator;
        }

        [HttpGet("generate-order-pdf/{orderId}")]
        public async Task<IActionResult> GenerateOrderPdf(int orderId)
        {
            // 1. Get data from the database
            var orderDetailDto = await _userOrderRepo.GetOrderDetailsByIdAsync(orderId);

            if (orderDetailDto == null)
            {
                return NotFound();
            }

            // 2. Load the HTML template file and populate it with data
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "PdfTemplates", "OrderDetailTemplate.cshtml");
            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            // 3. Populate the template with order data
            string populatedHtml = await PopulateHtmlTemplate(htmlTemplate, orderDetailDto);

            // 4. Use the service to generate the PDF (Select.Pdf - professional HTML to PDF)
            byte[] pdfBytes = _pdfGenerator.GeneratePdfForOrderList(populatedHtml);

            // 5. Return the file
            return File(pdfBytes, "application/pdf", $"Order_{orderId}.pdf");
        }

        /// <summary>
        /// Populates the HTML template with data from the Order.
        /// This method replaces placeholders with actual data, including a loop for order items.
        /// </summary>
        /// <param name="template">The HTML template as a string.</param>
        /// <param name="orderDetailDto">The Order containing the data.</param>
        /// <returns>A fully populated HTML string ready for PDF conversion.</returns>
        private async Task<string> PopulateHtmlTemplate(string template, Order orderDetailDto)
        {
            var htmlBuilder = new StringBuilder(template);

            // Calculate totals
            var totalItems = orderDetailDto.OrderDetail.Sum(x => x.Quantity);
            var subtotal = orderDetailDto.OrderDetail.Sum(x => x.Quantity * x.UnitPrice);

            // Replace basic summary placeholders
            htmlBuilder.Replace("{{OrderId}}", orderDetailDto.Id.ToString());
            htmlBuilder.Replace("{{CreateDate}}", orderDetailDto.CreateDate.ToString("MMM dd, yyyy"));
            htmlBuilder.Replace("{{TotalItems}}", totalItems.ToString());
            htmlBuilder.Replace("{{Subtotal}}", subtotal.ToString("0.00"));
            htmlBuilder.Replace("{{PaymentMethod}}", orderDetailDto.PaymentMethod ?? "N/A");

            // Customer information
            htmlBuilder.Replace("{{CustomerName}}", orderDetailDto.Name ?? "N/A");
            htmlBuilder.Replace("{{CustomerEmail}}", orderDetailDto.Email ?? "N/A");
            htmlBuilder.Replace("{{CustomerPhone}}", orderDetailDto.MobileNumber ?? "N/A");
            htmlBuilder.Replace("{{ShippingAddress}}", orderDetailDto.Address ?? "N/A");

            // Order status and payment status
            var orderStatus = orderDetailDto.OrderStatus?.StatusName ?? "Unknown";
            htmlBuilder.Replace("{{OrderStatus}}", orderStatus);
            htmlBuilder.Replace("{{OrderStatusClass}}", await GetStatusClass(orderStatus));

            var paymentStatus = orderDetailDto.IsPaid ? "Paid" : "Pending";
            htmlBuilder.Replace("{{PaymentStatus}}", paymentStatus);
            htmlBuilder.Replace("{{PaymentStatusClass}}", orderDetailDto.IsPaid ? "completed" : "pending");

            // Build the table rows dynamically using a loop
            var itemsBuilder = new StringBuilder();
            foreach (var item in orderDetailDto.OrderDetail)
            {
                if (item.Book != null)
                {
                    itemsBuilder.AppendLine("<tr>");
                    itemsBuilder.AppendLine("<td>");
                    itemsBuilder.AppendLine($"    <div class=\"d-flex\">");
                    itemsBuilder.AppendLine($"        <div class=\"bg-light p-2 rounded me-2\">");
                    itemsBuilder.AppendLine($"            <span style=\"font-size: 1.2em;\">📚</span>");
                    itemsBuilder.AppendLine($"        </div>");
                    itemsBuilder.AppendLine($"        <div>");
                    itemsBuilder.AppendLine($"            <h6 class=\"mb-0\">{item.Book.BookName}</h6>");
                    if (!string.IsNullOrEmpty(item.Book.AuthorName))
                    {
                        itemsBuilder.AppendLine($"            <small class=\"text-muted\">by {item.Book.AuthorName}</small>");
                    }
                    itemsBuilder.AppendLine($"        </div>");
                    itemsBuilder.AppendLine($"    </div>");
                    itemsBuilder.AppendLine("</td>");

                    var genreName = item.Book.Genre?.GenreName ?? "Unknown";
                    itemsBuilder.AppendLine($"<td><span class=\"badge bg-light text-dark\">{genreName}</span></td>");
                    itemsBuilder.AppendLine($"<td class=\"text-end\">${item.UnitPrice.ToString("0.00")}</td>");
                    itemsBuilder.AppendLine($"<td class=\"text-center\">{item.Quantity}</td>");
                    itemsBuilder.AppendLine($"<td class=\"text-end fw-bold\">${(item.Quantity * item.UnitPrice).ToString("0.00")}</td>");
                    itemsBuilder.AppendLine("</tr>");
                }
            }

            // Replace the {{OrderItems}} placeholder with the generated table rows
            htmlBuilder.Replace("{{OrderItems}}", itemsBuilder.ToString());

            return htmlBuilder.ToString();
        }

        /// <summary>
        /// Gets the CSS class for order status badges
        /// </summary>
        /// <param name="status">The order status name</param>
        /// <returns>The appropriate CSS class for the status badge</returns>
        private async Task<string> GetStatusClass(string status)
        {
            try
            {
                var orderStatuses = await _userOrderRepo.GetOrderStatuses();
                var matchedStatus = orderStatuses.FirstOrDefault(s => s.StatusName!.Equals(status, StringComparison.OrdinalIgnoreCase));

                return matchedStatus?.StatusName?.ToLower() switch
                {
                    "pending" => "pending",
                    "completed" => "completed",
                    "cancelled" => "cancelled",
                    "processing" => "pending",
                    "shipped" => "completed",
                    "delivered" => "completed",
                    _ => "pending"
                };
            }
            catch (Exception)
            {
                // Fallback to default if database query fails
                return status?.ToLower() switch
                {
                    "pending" => "pending",
                    "completed" => "completed",
                    "cancelled" => "cancelled",
                    _ => "pending"
                };
            }
        }

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
