using System.ComponentModel.DataAnnotations;

namespace BookShopingCartMVC.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string? StatusName { get; set; }
    }
}
