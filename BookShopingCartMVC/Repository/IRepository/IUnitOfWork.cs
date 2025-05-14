namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IBookRepository Book { get; set; }
        public IGenreRepository Genre { get; set; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
