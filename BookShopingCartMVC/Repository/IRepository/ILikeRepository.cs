namespace BookShopingCartMVC.Repository.IRepository
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Book>> GetAsync();
        Task<int> AddOrDeleteAsync(int BookId);
        Task<int> GetLikeCountAsync();
        string GetUserId();
    }
}
