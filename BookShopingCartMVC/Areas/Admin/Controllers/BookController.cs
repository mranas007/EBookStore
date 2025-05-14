using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using BookShopingCartMVC.Services;
using BookShopingCartMVC.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IFileServices _fileServices;
        private readonly IGenreRepository _genreRepo;
        public BookController(IBookRepository bookRepository, IFileServices fileServices, IGenreRepository genreRepository)
        {
            _bookRepo = bookRepository;
            _fileServices = fileServices;
            _genreRepo = genreRepository;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookRepo.GetBooksAsync();
            return View(books);
        }
        public async Task<IActionResult> BookDetails(int id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            return View(book);
        }
        public async Task<IActionResult> AddBook()
        {
            var genreSelectList = (await _genreRepo.GetAllAsync())
                .Select(genre => new SelectListItem
                {
                    Text = genre.GenreName,
                    Value = genre.Id.ToString()
                });

            BookDto bookToAdd = new BookDto { GenreList = genreSelectList };
            return View(bookToAdd);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookDto bookToAdd)
        {
            var genreSelectList = (await _genreRepo.GetAllAsync())
                .Select(genre => new SelectListItem
                {
                    Text = genre.GenreName,
                    Value = genre.Id.ToString()
                });
            bookToAdd.GenreList = genreSelectList;

            if (!ModelState.IsValid)
            {
                return View(bookToAdd);
            }

            try
            {
                if (bookToAdd.ImageFile is not null)
                {
                    if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image can not exceed 1 MB");
                    }
                    string[] allowedExtension = { "jpeg", "jpg", "png" };
                    string imageName = await _fileServices.SaveFileAsync(bookToAdd.ImageFile, allowedExtension);
                    bookToAdd.Image = imageName;
                }

                Book book = new Book
                {
                    Id = bookToAdd.Id,
                    BookName = bookToAdd.BookName,
                    AuthorName = bookToAdd.AuthorName,
                    Image = bookToAdd.Image,
                    Description = bookToAdd.Description,
                    GenreId = bookToAdd.GenreId,
                    Price = bookToAdd.Price,
                };
                await _bookRepo.AddAsync(book);
                TempData["success"] = "Book is added Successfully";
                return RedirectToAction(nameof(Index));

            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;
                return View(bookToAdd);
            }
            catch (FileNotFoundException ex)
            {
                TempData["error"] = ex.Message;
                return View(bookToAdd);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(bookToAdd);
            }

        }

        public async Task<IActionResult> UpdateBook(int id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book is null)
            {
                TempData["errorMessage"] = $"This Book does not found";
                return RedirectToAction(nameof(Index));
            }
            var genreSelectList = (await _genreRepo.GetAllAsync()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == book.GenreId
            });

            BookDto bookToUpdate = new BookDto
            {
                Id = book.Id,
                BookName = book.BookName,
                AuthorName = book.AuthorName,
                Price = book.Price,
                Description = book.Description,
                Image = book.Image,
                GenreId = book.GenreId,
                GenreList = genreSelectList,
            };

            return View(bookToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookDto bookToUpdate)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "model isn't valid";
                return View(bookToUpdate);
            }
            try
            {
                // save the new image file and delete the existing image.
                if (bookToUpdate.ImageFile is not null)
                {
                    if (bookToUpdate.Image is not null)
                    {
                        _fileServices.DeteleFile(bookToUpdate.Image);
                    }
                    string[] allowedExtension = { "jpeg", "jpg", "png" };
                    var fileName = await _fileServices.SaveFileAsync(bookToUpdate.ImageFile, allowedExtension);
                    bookToUpdate.Image = fileName;
                }

                Book book = new Book
                {
                    Id = bookToUpdate.Id,
                    BookName = bookToUpdate.BookName,
                    AuthorName = bookToUpdate.AuthorName,
                    Price = bookToUpdate.Price,
                    Description = bookToUpdate.Description,
                    Image = bookToUpdate.Image,
                    GenreId = bookToUpdate.GenreId,
                };

                await _bookRepo.UpdateAsync(book);
                TempData["success"] = "Book is updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "something went wrong" + ex;
                return View(bookToUpdate);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var existingBook = await _bookRepo.GetByIdAsync(id);
                if (existingBook is null)
                {
                    TempData["error"] = "Book isn't Exist.";
                    return RedirectToAction(nameof(Index));
                }
                if (existingBook.Image is not null)
                {
                    _fileServices.DeteleFile(existingBook.Image);
                }
                await _bookRepo.RemoveAsync(existingBook);
                TempData["success"] = "Book is deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"somthing went wrong {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
