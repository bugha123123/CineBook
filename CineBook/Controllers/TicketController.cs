using CineBook.Interface;
using CineBook.Services;
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

        public IActionResult TicketReservation()
        {
            return View();
        }
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
        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] List<int> seatIds)
        {
            try
            {
                if (seatIds == null || seatIds.Count == 0)
                    return BadRequest("No seats selected.");

                await ticketService.BookTicket(seatIds);
                return Ok(); // Return success
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 error
             
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }
    }
}
