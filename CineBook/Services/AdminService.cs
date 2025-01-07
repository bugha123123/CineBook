using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CineBook.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContextion _DBContext;
        private readonly IAuthService _AuthService;
        private readonly UserManager<User> _UserManager;
        public AdminService(AppDbContextion dBContext, IAuthService authService, UserManager<User> userManager)
        {
            _DBContext = dBContext;
            _AuthService = authService;
            _UserManager = userManager;
        }

        public async Task<int> GetTotalRevenue()
        {
           
            var bookingsWithSeats = await _DBContext.Bookings
                .Include(b => b.BookedSeats) 
                .ToListAsync();

            
            int totalRevenue = bookingsWithSeats
                .SelectMany(b => b.BookedSeats)
                .Sum(seat => seat.SeatPrice); 

            return totalRevenue;
        }


        public async Task<int> GetTotalTicketsSold()
        {
           
            var bookingsWithSeats = await _DBContext.Bookings
                .Include(b => b.BookedSeats) 
                .ToListAsync(); 

          
            int totalTicketsSold = bookingsWithSeats
                .Sum(b => b.BookedSeats.Count);  

            return totalTicketsSold;
        }

        public async Task<List<User>> GetTotalUsers()
        {
            var loggedInUser =await _AuthService.GetLoggedInUserAsync();
            var user = await _DBContext.Users.Where(x => x.UserName != loggedInUser.UserName).ToListAsync();
            return user;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _UserManager.FindByIdAsync(userId);

            if (user != null)
            {
              
                var bookings = await _DBContext.Bookings.Where(b => b.UserId == userId).ToListAsync();

                foreach (var booking in bookings)
                {
                    booking.UserId = null; 
                    _DBContext.Bookings.Update(booking);
                }

                await _UserManager.DeleteAsync(user);

                // Save changes
                await _DBContext.SaveChangesAsync();
            }
        }

        public async Task AddNewAdmin(string UserName, string Gmail, string Role)
        {
            
            var user = new User { UserName = UserName, Email = Gmail };

           
            var result = await _UserManager.CreateAsync(user, "Admin123"); // Use a strong default password

            if (result.Succeeded)
            {
                // If user creation is successful, add the role to the user
               await _UserManager.AddToRoleAsync(user, Role);

          
            }
      
        }

        public async Task<List<Seat>> SearchSeatsByMovies(int MovieId)
        {
            if (MovieId == null)
            {
                return new List<Seat>(); 
            }

          
               
                var seats = await _DBContext.Seats
                    .Where(s => s.MovieId == MovieId)
                    .ToListAsync();

                return seats;
          
        }


        public async Task<List<Movie>> GetAllMovies()
        {
            var TotalMovies = await _DBContext.Movies.AsNoTracking().ToListAsync();
            return TotalMovies;
        }

        public async Task<List<Booking>> GetAllBookings()
        {
            var TotalBookings = await _DBContext.Bookings.Include(x => x.movie).ToListAsync();
            return TotalBookings;
        }
    }
}
