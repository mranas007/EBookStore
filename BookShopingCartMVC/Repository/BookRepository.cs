using BookShopingCartMVC.Common;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace BookShopingCartMVC.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // this repo method used in Admin
        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            var books = await _context.Books
                .Include(a => a.Genre)
                .ToListAsync();
            return books;
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync(string sTerm = "", int genreId = 0, PaginationParams? paginationParams = null)
        {
            sTerm = sTerm?.ToLower() ?? string.Empty;
            var userId = GetUserId();

            var query = _context.Books
                .Include(b => b.Genre)
                .GroupJoin(_context.Likes.Where(l => l.UserId == userId), // Filter likes by current user
                           book => book.Id,
                           like => like.BookId,
                           (book, likes) => new { Book = book, Likes = likes })
                .SelectMany(x => x.Likes.DefaultIfEmpty(), // Left join with likes
                            (x, like) => new Book
                            {
                                Id = x.Book.Id,
                                BookName = x.Book.BookName,
                                AuthorName = x.Book.AuthorName,
                                Description = x.Book.Description,
                                Price = x.Book.Price,
                                Image = x.Book.Image,
                                GenreId = x.Book.GenreId,
                                GenreName = x.Book.Genre!.GenreName,
                                IsLike = like != null // Check if a like exists for this book by the user
                            })
                .Where(b => (string.IsNullOrWhiteSpace(sTerm) || (b.BookName != null && b.BookName.ToLower().Contains(sTerm)))
                            && (genreId == 0 || b.GenreId == genreId));

            if (paginationParams != null)
            {
                query = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                             .Take(paginationParams.PageSize);
            }

            return await query.ToListAsync();
        }


        public async Task<Book> GetByIdAsync(int id)
        {
            try
            {
                Book? book = await _context.Books
                  .Include(g => g.Genre)
                  .FirstOrDefaultAsync(x => x.Id == id);
                return book!;

            }
            catch (Exception ex)
            {
                throw new Exception("something went wrong: " + ex);
            }
        }

        public async Task UpdateAsync(Book book)
        {
            var bookId = _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Book book)
        {
            _context.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm?.ToLower() ?? string.Empty;

            var query = _context.Books
                .Where(b => (string.IsNullOrWhiteSpace(sTerm) || (b.BookName != null && b.BookName.ToLower().Contains(sTerm)))
                            && (genreId == 0 || b.GenreId == genreId));

            return await query.CountAsync();
        }

        private string GetUserId()
        {
            var pranciple = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(pranciple!)!;
        }

    }
}
