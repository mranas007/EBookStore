using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace BookShopingCartMVC.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public BookRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
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

        public async Task<IEnumerable<Book>> GetAllAsync(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm?.ToLower() ?? string.Empty;

            // Start with the books and join with genres
            var query = from book in _context.Books
                        join genre in _context.Genres on book.GenreId equals genre.Id
                        where
                            (string.IsNullOrWhiteSpace(sTerm) || (book.BookName != null && book.BookName.ToLower().Contains(sTerm)))
                            && (genreId == 0 || book.GenreId == genreId)
                        select new
                        {
                            Book = book,
                            Genre = genre
                        };

            // Execute the initial query to get the books
            var bookData = await query.ToListAsync();
            var userId = GetUserId();
            // Now process the like information
            var result = new List<Book>();

            foreach (var item in bookData)
            {
                // Check if there's a like for this book by the current user (if userId is provided)
                bool isLiked = false;
                isLiked = await _context.Likes
                    .AnyAsync(like => like.BookId == item.Book.Id && like.UserId == userId);

                // Create the book object with all needed properties
                result.Add(new Book
                {
                    Id = item.Book.Id,
                    BookName = item.Book.BookName,
                    AuthorName = item.Book.AuthorName,
                    Description = item.Book.Description,
                    Price = item.Book.Price,
                    Image = item.Book.Image,
                    GenreId = item.Book.GenreId,
                    GenreName = item.Genre.GenreName,
                    IsLike = isLiked
                });
            }

            return result;
        }


        public async Task<Book> GetByIdAsync(int id)
        {
            try
            {
                Book? book = await _context.Books
                  .Include(g => g.Genre)
                  .FirstOrDefaultAsync(x => x.Id == id);
                return book;

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

        private string GetUserId()
        {
            var pranciple = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(pranciple);
        }

    }
}
