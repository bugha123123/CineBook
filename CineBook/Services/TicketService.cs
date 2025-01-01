using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContextion _dbcontext;
        private readonly IAuthService _authService;
        public TicketService(AppDbContextion dbcontext, IAuthService authService)
        {
            _dbcontext = dbcontext;
            _authService = authService;
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
               
            };

            await _dbcontext.Bookings.AddAsync(booking);
            await _dbcontext.SaveChangesAsync();
        }


        public async Task<List<Movie>> GetMovies()
        {
            return await _dbcontext.Movies.ToListAsync();
        }

        public async Task<List<Seat>> GetSeatsByMovieId(int MovieId)
        {
            return await _dbcontext.Seats.Where(x => x.MovieId == MovieId).ToListAsync();
        }
    }
}
