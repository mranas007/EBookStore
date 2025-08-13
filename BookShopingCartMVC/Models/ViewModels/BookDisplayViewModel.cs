namespace BookShopingCartMVC.Models.ViewModels
{
    public class BookDisplayViewModel
    {
        public IEnumerable<Genre>? Genres { get; set; }
        public IEnumerable<Book>? Books { get; set; }

        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalBooks { get; set; }
    }
}
