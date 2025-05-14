using System.ComponentModel.DataAnnotations;

namespace BookShopingCartMVC.Models.DTOs
{
    public class StockDto
    {
        public int BookId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non negative value.")]
        public int Quantity { get; set; }
    }
}
