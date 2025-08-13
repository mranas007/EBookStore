using BookShopingCartMVC.Repository.IRepository;

namespace BookShopingCartMVC.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Genre genre)
        {
            _context.Genres.Remove(genre);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            Genre? genre = await _context.Genres.FindAsync(id);
            return genre!;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }



    }
}
