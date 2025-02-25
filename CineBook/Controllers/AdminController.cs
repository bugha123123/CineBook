﻿using CineBook.Interface;
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
        private readonly IChatService _chatService;
        public AdminController(IAdminService adminService, UserManager<User> userManager, IChatService chatService)
        {
            _adminService = adminService;
            _userManager = userManager;
            _chatService = chatService;
        }
        [Authorize(Roles ="Admin")]
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

        public async Task<IActionResult> SupportTicketsDetails(string TicketId)
        {
            // Retrieve the ticket using the TicketId
            var FoundTicket = await _adminService.GetSupportTicketById(TicketId);

            // If no ticket is found, redirect to the Index page
            if (FoundTicket is  null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(FoundTicket);
        }

        public async Task<IActionResult> allsupporttickets()
        {

            return View();
        }


        public async Task<IActionResult> FailedVerificationError()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageBookings()
        {
            var  TotalBookings = await _adminService.GetAllBookings();

            return View(TotalBookings);
        }

        [Authorize(Roles = "Admin")]
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
                user.IsVerified = false;
                user.VerifiedAt = DateTime.Now;  
                user.VerificationToken = null;  

                await _userManager.UpdateAsync(user); 

                TempData["Message"] = "Email successfully verified!";
                return RedirectToAction("OwnerShip", "Admin");
            }
            else
            {
                TempData["Error"] = "Verification failed.";
                return RedirectToAction("FailedVerificationError", "Admin");
            }
        }

        public async Task<IActionResult> Admin_SendResetPasswordGmail(string Gmail)
        {
            if (ModelState.IsValid)
            {
                await _adminService.SendResetPasswordEmail(Gmail);
                return RedirectToAction("MainAdminPage", "Admin");
            }
            return View();
        }

        public async Task<IActionResult> Admin_ChangeStatusOfChat(string ChatId, Chat.ChatStatus status)
        {
            if (ModelState.IsValid)
            {
                await _adminService.UpdateChatStatus(ChatId, status);
                return RedirectToAction("MainAdminPage", "Admin");
            }
            return View();
        }
        public async Task<IActionResult> Admin_JoinChatAndChangeStatus(string ChatId)
        {
            if (ModelState.IsValid)
            {
                await _adminService.JoinChat(ChatId);
                return RedirectToAction("compactchat", "Help", new {ChatId, Role = "Admin"});
            }
            return View();
        }

        public async Task<IActionResult> Admin_SendChatMessage(string Comment, string TicketId)
        {
            if (ModelState.IsValid)
            {
                // Add the comment using your service
                await _adminService.AddCommentChangeStatus(Comment, TicketId);

                // Redirect to the same ticket details page with the same TicketId
                return RedirectToAction("SupportTicketsDetails", "Admin", new { TicketId = TicketId });
            }
            return View();
        }

        public async Task<IActionResult> Admin_ResolveSupportTicket(string TicketId)
        {
            if (ModelState.IsValid)
            {
                // Add the comment using your service
                await _adminService.ResolveTicket(TicketId);

                // Redirect to the same ticket details page with the same TicketId
                return RedirectToAction("help", "Help", new { TicketId = TicketId });
            }
            return View();
        }

    }
}
