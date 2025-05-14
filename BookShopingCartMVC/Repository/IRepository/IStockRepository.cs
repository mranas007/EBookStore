using BookShopingCartMVC.Models.DTOs;

namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IStockRepository
    {
        Task<Stock?> GetStcokById(int bookId);
        Task ManageTask(StockDto stockToManage);
        Task<IEnumerable<StockViewModel>> GetStcoks(string sterm = "");
    }
}