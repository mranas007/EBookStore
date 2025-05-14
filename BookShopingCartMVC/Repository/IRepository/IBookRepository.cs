namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<IEnumerable<Book>> GetAllAsync(string sTerm, int genreId);
        Task<Book> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task RemoveAsync(Book book);
        Task UpdateAsync(Book book);
    }
}
