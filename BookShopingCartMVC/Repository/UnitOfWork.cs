using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace BookShopingCartMVC.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBookRepository Book { get; set; }
        public IGenreRepository Genre { get; set; }

        public UnitOfWork(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            Book = new BookRepository(context, httpContextAccessor, userManager);
            Genre = new GenreRepository(context);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
