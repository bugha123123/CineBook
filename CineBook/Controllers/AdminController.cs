using CineBook.Interface;
using CineBook.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> MainAdminPage()
        {
            var movies = await _adminService.GetAllMovies();
            ViewData["Movies"] = movies; 

            return View();
        }

        public async Task<IActionResult> AddNewAdmin(string UserName, string Gmail, string Role)
        {
            await _adminService.AddNewAdmin(UserName, Gmail, Role);
            return RedirectToAction("MainAdminPage", "Admin");
        }

        public async Task<IActionResult> SearchSeats(int movieId)
        {
            var seats = await _adminService.SearchSeatsByMovies(movieId);  // Use your service to fetch the seats
            return Json(seats);  // Return seats as JSON
        }




    }
}
