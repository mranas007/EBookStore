using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace BookShopingCartMVC.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeRepository(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        // fetch the users fav book
        public async Task<IEnumerable<Book>> GetAsync()
        {
            try
            {
                var favBook = await (from book in _context.Books
                                     join like in _context.Likes
                                     on book.Id equals like.BookId
                                     where like.UserId == GetUserId()
                                     select book).ToListAsync();
                return favBook;
            }
            catch (Exception ex)
            {
                throw new Exception("something went wrong to fetch the favourite" + ex);
            }
        }

        //get user like 
        public async Task<int> GetLikeCountAsync()
        {
            var count = await _context.Likes.Where(l => l.UserId == GetUserId()).CountAsync();

            if (count > 0)
                return count;

            return 0;
        }

        // add the users fav book
        public async Task<int> AddOrDeleteAsync(int BookId)
        {
            try
            {
                // When finding a Like, we need to search by both BookId and UserId
                var userId = GetUserId();
                var result = await _context.Likes
                    .FirstOrDefaultAsync(l => l.BookId == BookId && l.UserId == userId);

                // found then delete
                if (result is not null)
                {
                    _context.Likes.Remove(result);
                    await _context.SaveChangesAsync();
                    return 1;
                }

                Like like = new Like
                {
                    BookId = BookId,
                    UserId = userId
                };
                // create if not found
                await _context.Likes.AddAsync(like);
                var check = await _context.SaveChangesAsync();
                return 2;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AddOrDeleteAsync: {ex.Message}");
            }
        }

        // get the user id
        public string GetUserId()
        {
            var pranciple = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(pranciple!)!;
        }

    }
}
