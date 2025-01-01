using CineBook.Models;

namespace CineBook.Interface
{
    public interface ITicketService
    {

        Task BookTicket(List<int> SeatId);

        Task<List<Movie>> GetMovies();

        Task<List<Seat>> GetSeatsByMovieId(int MovieId);

       Task<Movie> GetMovieById(int MovieId);

        Task<List<Booking>> GetBookingHistoryForUser();
    }
}
