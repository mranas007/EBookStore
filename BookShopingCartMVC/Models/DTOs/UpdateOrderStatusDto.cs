using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShopingCartMVC.Models.DTOs
{
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        public IEnumerable<SelectListItem>? OrderStatusList { get; set; }
    }
}
