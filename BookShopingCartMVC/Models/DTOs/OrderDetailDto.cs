namespace BookShopingCartMVC.Models.DTOs
{
    public class OrderDetailDto
    {
        public int DivId { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
