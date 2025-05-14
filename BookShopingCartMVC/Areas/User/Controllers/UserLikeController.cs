using System.Text.Json;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopingCartMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class UserLikeController : Controller
    {
        private readonly ILikeRepository _likeRepo;
        public UserLikeController(ILikeRepository likeRepository)
        {
            _likeRepo = likeRepository;
        }

        // return view()
        public async Task<IActionResult> Favourite(int id)
        {
            try
            {
                var favBook = await _likeRepo.GetAsync();
                return View(favBook);
                //var options = new JsonSerializerOptions
                //{
                //    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                //};
                //return Json(favBook, options);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Success = false,
                    ExceptionType = ex.GetType().Name,
                    Message = ex.Message,
                };

                return BadRequest(errorResponse);
            }
        }

        // add and delete for book favourite - API
        [HttpPost]
        public async Task<IActionResult> AddOrDelete(int id)
        {
            try
            {
                // if id is null return bad request
                if (id.ToString() is null) BadRequest(new { message = "id is empty", success = false, Book_id = id });

                var check = await _likeRepo.AddOrDeleteAsync(id);

                // check if removed
                if (check == 1)
                {
                    return Ok(new { message = "Successfully Removed", success = true, Book_id = id });
                }
                // check if created
                else if (check == 2)
                {
                    return Ok(new { message = "Successfully Created", success = true, Book_id = id });
                }

                // else something wrong
                return BadRequest(new { message = "Operation Failed", success = false, Book_id = id });
            }
            catch (Exception ex)
            {
                var exception = new
                {
                    Message = ex.Message,
                    Success = false,
                    ExceptionType = ex.GetType().Name,
                };
                return BadRequest(exception);
            }
        }
    }
}
