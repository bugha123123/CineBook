using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace CineBook.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContextion _dbcontext;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public TicketService(AppDbContextion dbcontext, IAuthService authService, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _authService = authService;
            _emailService = emailService;
        }

        public async Task BookTicket(List<int> seatIds)
        {
            var loggedInUser = await _authService.GetLoggedInUserAsync();

            if (seatIds == null || !seatIds.Any())
                throw new ArgumentException("No seats provided for booking.");

            var seats = await _dbcontext.Seats
                .Where(s => seatIds.Contains(s.Id))
                .ToListAsync();

            foreach (var seat in seats)
            {
                if (!seat.IsAvailable)
                    throw new Exception($"Seat with ID {seat.Id} is already booked.");

                // Mark the seat as booked
                seat.IsAvailable = false;
               
            }

            // Create a single booking for all seats
            var booking = new Booking
            {
                UserId = loggedInUser.Id,
                BookedSeats = seats,
                MovieId = seats.First().MovieId,
                movie = seats.First().Movie,
                user = loggedInUser,
                BookedSeatNumbers = seatIds,
                IsCompleted = true,
                BookedAt = DateTime.Now
               
              
               
               
               
            };

            await _dbcontext.Bookings.AddAsync(booking);
            await _dbcontext.SaveChangesAsync();
            string emailBody = @"
    <html>
        <head>
            <style>
                body {
                    font-family: 'Arial', sans-serif;
                    background-color: #f9f9f9;
                    margin: 0;
                    padding: 0;
                }
                .container {
                    width: 100%;
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #fff;
                    border-radius: 8px;
                    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
                    padding: 20px;
                }
                .header {
                    text-align: center;
                    padding: 10px 0;
                    border-bottom: 2px solid #f2f2f2;
                }
                .header h1 {
                    color: #007BFF;
                    font-size: 36px;
                    margin: 0;
                }
                .body-content {
                    padding: 20px;
                }
                .body-content h2 {
                    font-size: 24px;
                    color: #333;
                    margin-bottom: 10px;
                }
                .ticket-details {
                    margin-bottom: 20px;
                }
                .ticket-details p {
                    font-size: 16px;
                    color: #555;
                    margin: 8px 0;
                }
                .footer {
                    text-align: center;
                    padding: 10px;
                    background-color: #007BFF;
                    color: white;
                    font-size: 14px;
                    border-radius: 8px;
                }
                .footer a {
                    color: #ffffff;
                    text-decoration: none;
                }
                .button {
                    background-color: #28a745;
                    color: white;
                    padding: 12px 20px;
                    text-align: center;
                    border-radius: 5px;
                    font-size: 16px;
                    cursor: pointer;
                    text-decoration: none;
                }
                .button:hover {
                    background-color: #218838;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>CineBook</h1>
                    <p>Your Booking is Confirmed!</p>
                </div>
                <div class='body-content'>
                    <h2>Booking Summary</h2>
                  
                    <a class='button'>View Booking</a>
                </div>
                <div class='footer'>
                    <p>Thank you for booking with CineBook! If you have any questions, feel free to contact us.</p>
                    <p><a href = 'mailto:support@cinebook.com' > support@cinebook.com</a></p>
                </div>
            </div>
        </body>
    </html>";
            await _emailService.SendEmailAsync(loggedInUser.Email, "Your Ticket Booking Confirmation - CineBook", emailBody);
        }

        public async Task<List<Booking>> GetBookingHistoryForUser()
        {
            var User = await _authService.GetLoggedInUserAsync();
                
                var BookingHistory = await _dbcontext.Bookings.Include(x => x.movie).Include( x => x.BookedSeats).Where(x => x.UserId == User.Id).ToListAsync();

            return BookingHistory;
        }

        public async Task<List<Movie>> GetComingNowMovies()
        {
            var currentDateTime = DateTime.Now;

            // Define a time window (e.g., movies starting in the next 30 minutes or already started)
            var windowStartTime = currentDateTime.AddMinutes(-30); // 30 minutes before now
            var windowEndTime = currentDateTime.AddMinutes(30); // 30 minutes after now

            // Fetch movies where ShowTime is within the time window
            var nowMovies = await _dbcontext.Movies
                .Where(movie => movie.ShowTime >= windowStartTime && movie.ShowTime <= windowEndTime)
                .ToListAsync();

            return nowMovies;
        }

        public async Task<Movie> GetMovieById(int MovieId)
        {
            return await _dbcontext.Movies.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == MovieId);
        }

        public async Task<List<Movie>> GetMovies()
        {
            return await _dbcontext.Movies.ToListAsync();
        }

        public async Task<List<Seat>> GetSeatsByMovieId(int MovieId)
        {
            return await _dbcontext.Seats.Where(x => x.MovieId == MovieId).ToListAsync();
        }

        public async Task<List<Movie>> GetSoonComingMovies()
        {
            var currentDate = DateTime.Now;

           
            var upcomingMovies = await _dbcontext.Movies
                .Where(movie => movie.ShowTime > currentDate && movie.ShowTime <= currentDate.AddDays(30)) // Filter for upcoming movies within 30 days
                .ToListAsync();

            return upcomingMovies;
        }

        public async Task RemoveBooking(int BookingId)
        {
            var BookingToRemove = await _dbcontext.Bookings
                                                  .Include(x => x.BookedSeats)  
                                                  .FirstOrDefaultAsync(x => x.Id == BookingId);

            if (BookingToRemove == null)
            {
                return; 
            }

            // Flag each booked seat as unavailable
            foreach (var seat in BookingToRemove.BookedSeats)
            {
                seat.IsAvailable = true;
                
            }

            // Remove the booking from the database
            _dbcontext.Bookings.Remove(BookingToRemove);

            await _dbcontext.SaveChangesAsync();
        }

    }
}
