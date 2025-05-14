using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace BookShopingCartMVC.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStcokById(int bookId) =>
            await _context.Stocks.FirstOrDefaultAsync(s => s.BookId == bookId);


        public async Task ManageTask(StockDto stockToManage)
        {
            var existingStock = await GetStcokById(stockToManage.BookId);
            if (existingStock is null)
            {
                var stock = new Stock { BookId = stockToManage.BookId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<StockViewModel>> GetStcoks(string sterm = "")
        {
            var stocks = await (from book in _context.Books
                                join stock in _context.Stocks
                                on book.Id equals stock.BookId
                                into book_stock
                                from bookStock in book_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sterm) || book.BookName.ToLower().Contains(sterm.ToLower())
                                select new StockViewModel
                                {
                                    BookId = book.Id,
                                    BookName = book.BookName,
                                    Quantity = bookStock == null ? 0 : bookStock.Quantity
                                }).ToListAsync();
            return stocks;
        }

    }
}
