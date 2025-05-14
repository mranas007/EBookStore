using BookShopingCartMVC.Models.DTOs;

namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrderAsync(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusDto model);
        Task TogglePaymentStatus(int orderId);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses( );
    }
}
