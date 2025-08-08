using Microsoft.AspNetCore.Mvc.Rendering;
using BookShopingCartMVC.Models;

namespace BookShopingCartMVC.Models.ViewModels
{
    public class Orders_StatusesViewModel
    {
        public IEnumerable<Order>? Orders { get; set; }
        public List<SelectListItem>? OrderStatus { get; set; }
        public string? Status { get; set; }

    }
}
