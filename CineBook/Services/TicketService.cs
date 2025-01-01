using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContextion _dbcontext;

        public TicketService(AppDbContextion dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task BookTicket(List<int> SeatId)
        {

            if (SeatId.Count == 0)
            {
                throw new ArgumentException("No seats provided for booking.");
            }

            var seats = await _dbcontext.Seats.Where(s => SeatId.Contains(s.Id)).ToListAsync();
            foreach (var seatId in SeatId)
            {
                var seat = seats.FirstOrDefault(s => s.Id == seatId);
                if (seat == null)
                {
                    throw new Exception($"Seat with ID {seatId} does not exist.");
                }

                if (!seat.IsAvailable)
                {
                    throw new Exception($"Seat with ID {seatId} is already booked.");
                }

                // Mark the seat as booked
                seat.IsAvailable = false;
            }
        }

        public async Task<List<Movie>> GetMovies()
        {
            return await _dbcontext.Movies.ToListAsync();
        }

        public async Task<List<Seat>> GetSeats()
        {
            return await _dbcontext.Seats.ToListAsync();
        }
    }
}
