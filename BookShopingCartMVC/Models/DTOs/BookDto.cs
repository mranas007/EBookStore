using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShopingCartMVC.Models.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [DisplayName("Book Name")]
        public string? BookName { get; set; }

        [Required]
        [MaxLength(40)]
        [DisplayName("Author Name")]
        public string? AuthorName { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; } // to hold the image path or URL
        public IFormFile? ImageFile { get; set; } // to hold the uploaded file

        [Required]
        public int GenreId { get; set; }

        [DisplayName("Genres")]
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
