using CineBook.Interface;
using CineBook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        [Authorize]
        public async Task<IActionResult> BookingHistory()
        {
            var BookingHistoryForUser = await ticketService.GetBookingHistoryForUser();
            return View(BookingHistoryForUser);
        }
        public async Task<IActionResult> MovieDetails(int MovieId)
        {
            var MovieById = await ticketService.GetMovieById(MovieId);  
            return View(MovieById);
        }
        public IActionResult Seats()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ComingSoon()
        {
            var result = await ticketService.GetSoonComingMovies();
            return View(result);
        }
        [Authorize]
        public async Task<IActionResult> ComingNow()
        {
            var result = await ticketService.GetComingNowMovies();
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] List<int> seatIds)
        {
            try
            {
                if (seatIds == null || seatIds.Count == 0)
                    return BadRequest("No seats selected.");

                await ticketService.BookTicket(seatIds);
                return Ok(); 
            }
            catch (Exception ex)
            {
             
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }

        public async Task<IActionResult> RemoveBooking(int BookingId)
        {
            try
            {
            await ticketService.RemoveBooking(BookingId);
                return RedirectToAction("BookingHistory", "Ticket");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error. Please try again.{ex}");
            }
        }
    }
}
