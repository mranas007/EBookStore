using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopingCartMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }
        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }
        [Required]
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        [NotMapped]
        public bool? IsLike { get; set; } = false; 

        [NotMapped]
        public string? GenreName { get; set; }
        [NotMapped]
        public int Quantity { get; set; }

        public List<OrderDetail>? OrderDetail { get; set; }
        public List<CartDetail>? CartDetail { get; set; }
        public Stock? Stock { get; set; }
        public List<Like>? Like { get; set; }
    }
}