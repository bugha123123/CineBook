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
        public async Task<IActionResult> ManageBookings()
        {
            var  TotalBookings = await _adminService.GetAllBookings();

            return View(TotalBookings);
        }

        public async Task<IActionResult> GenerateReport(int? dateRange)
        {

            int lastNDays = dateRange ?? 30; 

           
        
           
            var salesReport = await _adminService.GenerateSalesReport(lastNDays);
            return View(salesReport);
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



        public async Task<IActionResult> RemoveBooking(int BookingId)
        {
            if (ModelState.IsValid)
            {
                await _adminService.RemoveBooking(BookingId);
                return RedirectToAction("ManageBookings","Admin" );
            }
            return RedirectToAction("ManageBookings", "Admin");
        }
    }
}
