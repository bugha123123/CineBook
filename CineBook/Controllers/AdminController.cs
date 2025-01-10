using CineBook.Interface;
using CineBook.Models;
using CineBook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<User> _userManager;

        public AdminController(IAdminService adminService, UserManager<User> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> MainAdminPage()
        {
            var movies = await _adminService.GetAllMovies();
            ViewData["Movies"] = movies; 

            return View();
        }

        public async Task<IActionResult> Ownership()
        {

            return View();
        }

        public async Task<IActionResult> FailedVerificationError()
        {

            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ManageBookings()
        {
            var  TotalBookings = await _adminService.GetAllBookings();

            return View(TotalBookings);
        }

        [Authorize(Roles = "ADMIN")]
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

        public async Task<IActionResult> RemoveUser(string UserId)
        {
            if (ModelState.IsValid)
            {
                await _adminService.RemoveUser(UserId);
                return RedirectToAction("MainAdminPage", "Admin");
            }
            return RedirectToAction("MainAdminPage", "Admin");
        }

        public async Task<IActionResult> SendVerificationEmailToUser(string UserGmail)
        {
            var user = await _userManager.FindByEmailAsync(UserGmail);

            if (user != null)
            {
                // Generate a new token for the user
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Construct the verification URL with the token and userId
                string verificationUrl = $"http://localhost:7194/Admin/VerifyOwnership?userId={user.Id}&token={token}";

                // Send the verification email
                await _adminService.SendVerificationEmail(user.Email, user.Id);

                // Provide feedback to the admin
                TempData["Message"] = "Verification email sent successfully.";
                return RedirectToAction("MainAdminPage", "Admin");
            }
            else
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("MainAdminPage", "Admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyOwnership(string userId, string time)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Invalid verification link.";
                return RedirectToAction("Error", "Admin");
            }

            // Parse the time from the query parameter
            DateTime tokenGenerationTime;
            if (!DateTime.TryParse(time, out tokenGenerationTime))
            {
                TempData["Error"] = "Invalid verification link.";
                return RedirectToAction("Error", "Admin");
            }

            // Check if the token is expired (5 minutes limit)
            if ((DateTime.Now - tokenGenerationTime).TotalMinutes > 5)
            {
                TempData["Error"] = "Verification link has expired.";
                return RedirectToAction("Error", "Admin");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Error", "Admin");
            }

            // Verify the user's email using the token
            var result = await _userManager.ConfirmEmailAsync(user, user.VerificationToken);
            if (result.Succeeded)
            {
                user.IsVerified = true;
                user.VerifiedAt = DateTime.Now;  // Set the verification time
                user.VerificationToken = null;  // Clear the token after successful verification
                await _userManager.UpdateAsync(user); // Save changes to the database

                TempData["Message"] = "Email successfully verified!";
                return RedirectToAction("OwnerShip", "Admin");
            }
            else
            {
                TempData["Error"] = "Verification failed.";
                return RedirectToAction("FailedVerificationError", "Admin");
            }
        }




    }
}
