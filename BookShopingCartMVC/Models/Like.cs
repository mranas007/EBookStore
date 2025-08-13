using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopingCartMVC.Models
{
    public class Like
    {
        public int Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }
    }
}
