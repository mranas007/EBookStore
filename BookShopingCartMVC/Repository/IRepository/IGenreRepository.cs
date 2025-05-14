namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IGenreRepository
    {
        Task AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task<bool> RemoveAsync(Genre genre);
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre> GetByIdAsync(int id);
    }
}
