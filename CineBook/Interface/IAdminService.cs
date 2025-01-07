using CineBook.Models;

namespace CineBook.Interface
{
    public interface IAdminService
    { 
        Task<int> GetTotalRevenue();

        Task<int> GetTotalTicketsSold();

        Task<List<User>> GetTotalUsers();

        Task DeleteUserAsync(string userId);
        Task AddNewAdmin(string UserName, string Gmail, string Role);

        Task<List<Seat>> SearchSeatsByMovies(int MovieId);
        Task<List<Movie>> GetAllMovies();
    }
}
