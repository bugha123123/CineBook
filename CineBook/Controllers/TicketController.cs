using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Services;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService ticketService;
        private readonly AppDbContextion appDbContextion;
        public TicketController(ITicketService ticketService, AppDbContextion appDbContextion)
        {
            this.ticketService = ticketService;
            this.appDbContextion = appDbContextion;
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
        public async Task<IActionResult> bookingconfirmation(int BI)
        {
            var FoundTicket = await ticketService.GetBookedMovieById(BI);
            return View(FoundTicket);
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
        public async Task<IActionResult> DownloadTicketPDF(int BookingId)
        {
            var booking = await appDbContextion.Bookings
                .Include(b => b.movie)
                .Include(b => b.user)
                .FirstOrDefaultAsync(b => b.Id == BookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Title: Movie Ticket Confirmation
                document.Add(new Paragraph("Movie Ticket Confirmation")
                    .SetFontSize(22)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED));

                // Movie Information Section
                document.Add(new Paragraph($"Movie: {booking.movie.Title}")
                    .SetFontSize(16)
                    .SetBold()
                    .SetFontColor(ColorConstants.DARK_GRAY));

                document.Add(new Paragraph($"Genre: {booking.movie.Genre} | RunTime: {booking.movie.RunTime}")
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.GRAY));

                document.Add(new Paragraph($"Release Date: {booking.movie.ReleaseDate}")
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.GRAY));

                // Booking Details Section
                document.Add(new Paragraph($"Booking ID: {booking.Id}")
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK));

                document.Add(new Paragraph($"User: {booking.user.UserName}")
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK));

                document.Add(new Paragraph($"Email: {booking.user.Email}")
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK));

                // Seats Section
                document.Add(new Paragraph("Seats:")
                    .SetFontSize(14)
                    .SetBold()
                    .SetFontColor(ColorConstants.RED));

                foreach (var seat in booking.BookedSeatNumbers)
                {
                    document.Add(new Paragraph($"Seat: {seat}")
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.BLACK));
                }

                // Total Price Section
                document.Add(new Paragraph($"Total Price: ${booking.BookedSeatNumbers.Count() * 10}")
                    .SetFontSize(14)
                    .SetBold()
                    .SetFontColor(ColorConstants.RED));

                // Footer
                document.Add(new Paragraph("\n")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph("Please arrive 15 minutes before the showtime.")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.GRAY));

                document.Add(new Paragraph("No refunds after purchase.")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.GRAY));

                document.Close();

                var pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", $"Ticket_{booking.Id}.pdf");
            }
        }


    }
}
