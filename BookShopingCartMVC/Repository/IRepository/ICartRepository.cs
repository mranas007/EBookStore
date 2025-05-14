using BookShopingCartMVC.Models.DTOs;

namespace BookShopingCartMVC.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<int> AddItemAsync(int bookId, int quantity);
        Task<int> RemoveItemAsync(int bookId);
        Task<int> RemoveAllItemAsync();
        Task<ShoppingCart> GetAllCartAsync();
        Task<bool> DoCheckoutAsync(CheckoutModel model);
        Task<ShoppingCart> GetCartAsync();
        Task<int> GetCartItemCountAsync();
        string GetUserId();
    }
}
