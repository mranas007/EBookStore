using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;
        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepo = genreRepository;
        }

        public async Task<IActionResult> Index()
        {
            var geners = await _genreRepo.GetAllAsync();
            return View(geners);
        }

        public IActionResult AddGenre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(GenreDto genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }
            try
            {
                var newGenre = new Genre
                {
                    Id = genre.Id,
                    GenreName = genre.GenreName
                };
                await _genreRepo.AddAsync(newGenre);
                TempData["message"] = "Genre Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Genre Couldn't Added!";
                return View(genre);
            }
        }

        public async Task<IActionResult> UpdateGenre(int id)
        {
            var getGenre = await _genreRepo.GetByIdAsync(id);
            if (getGenre is null)
                return RedirectToAction(nameof(Index));
            GenreDto oldGenre = new GenreDto { Id = getGenre.Id, GenreName = getGenre.GenreName };
            return View(oldGenre);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGenre(GenreDto genre)
        {
            if (!ModelState.IsValid)
                return View(genre);
            try
            {
                var newGenre = new Genre
                {
                    Id = genre.Id,
                    GenreName = genre.GenreName
                };
                await _genreRepo.UpdateAsync(newGenre);
                TempData["message"] = "Genre added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Genre Couldn't update!";
                return View();
            };
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            Console.WriteLine(id);
            var existingGenre = await _genreRepo.GetByIdAsync(id);
            if (existingGenre is null)
            {
                TempData["message"] = "The Genre doesn't exist.";
                return RedirectToAction(nameof(Index));
            };
            try
            {
                var result = await _genreRepo.RemoveAsync(existingGenre);
                if (result)
                {
                    TempData["success"] = "Genre Deleted Successfully";
                }
                else
                {
                    TempData["error"] = "Genre Couldn't Deleted!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Genre Couldn't Deleted!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
