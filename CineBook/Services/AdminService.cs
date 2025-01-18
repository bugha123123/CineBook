using CineBook.ApplicationDbContext;
using CineBook.DTO;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net;

namespace CineBook.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContextion _DBContext;
        private readonly IAuthService _AuthService;
        private readonly UserManager<User> _UserManager;
        private readonly IEmailService _emailService;
        private readonly IChatService _chatService;
        public AdminService(AppDbContextion dBContext, IAuthService authService, UserManager<User> userManager, IEmailService emailService, IChatService chatService)
        {
            _DBContext = dBContext;
            _AuthService = authService;
            _UserManager = userManager;
            _emailService = emailService;
            _chatService = chatService;
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
            var user = await _DBContext.Users.Where(x => x.Id != loggedInUser.Id).ToListAsync();
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
            var TotalBookings = await _DBContext.Bookings.Include(x => x.movie).Include(x => x.user).ToListAsync();
            return TotalBookings;
        }
        public async Task<SalesReport> GenerateSalesReport(int lastNDays)
        {
            
            var startDate = DateTime.Now.Date.AddDays(-lastNDays);

            var bookings = await _DBContext.Bookings
                .Where(b => b.BookedAt >= startDate && b.BookedAt <= DateTime.Now)
                .Include(b => b.user) 
                .Include(b => b.movie)
                .Include(b => b.BookedSeats) 
                .ToListAsync();

            
            var totalBookedSeats = bookings.Sum(booking => booking.BookedSeats.Count);

            
            
            var totalRevenue = await GetTotalRevenue();

        

            return new SalesReport
            {
                DateRange = $"{startDate:yyyy-MM-dd} to {DateTime.Now:yyyy-MM-dd}",
                TotalBookedSeats = totalBookedSeats,
                TotalRevenue = totalRevenue,
              
            };
        }
        public async Task<Booking> GetBookingById(int BookingId)
        {
            var FoundBooking = await _DBContext.Bookings.FirstOrDefaultAsync(b => b.Id == BookingId);



            return FoundBooking;
        }
        public async Task RemoveBooking(int BookingId)
        {
            var BookingToRemove = await _DBContext.Bookings
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
            _DBContext.Bookings.Remove(BookingToRemove);

            await _DBContext.SaveChangesAsync();
        }

        public async Task RemoveUser(string UserId)
        {
            var FoundUser = await GetUserById(UserId);

            if (FoundUser == null)
                return;

            _DBContext.Users.Remove(FoundUser);
            await _DBContext.SaveChangesAsync();
          
        }

        public async Task<User> GetUserById(string UserId)
        {
            var FoundUser = await _UserManager.FindByIdAsync(UserId);

            return FoundUser;
        }

        public async Task SendVerificationEmail(string Gmail, string userId)
        {
            var user = await _UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var token = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
            user.VerificationToken = token;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            await _DBContext.SaveChangesAsync();

            var tokenGenerationTime = DateTime.Now;
            string verificationUrl = $"https://localhost:7194/Admin/VerifyOwnership?userId={userId}&token={user.VerificationToken}&time={tokenGenerationTime}";

            string body = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Verify Your Email</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f9f9f9;
                    margin: 0;
                    padding: 0;
                }
                .email-container {
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                }
                .email-header {
                    text-align: center;
                    color: #333;
                    font-size: 24px;
                    font-weight: bold;
                    margin-bottom: 20px;
                }
                .email-content {
                    text-align: center;
                    color: #555;
                    font-size: 16px;
                    margin-bottom: 20px;
                    line-height: 1.6;
                }
                .verify-button {
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 16px;
                    color: #ffffff;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                }
                .verify-button:hover {
                    background-color: #0056b3;
                }
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    Verify Your Email Address
                </div>
                <div class='email-content'>
                    <p>Hello,</p>
                    <p>We received a request to verify your email address. Please confirm ownership by clicking the button below:</p>
                    <a style='text-decoration:none;' href='" + verificationUrl + @"' class='verify-button'>Verify Email</a>
                    <p>If you didn't request this, you can ignore this email.</p>
                    <p>Thank you!</p>
                </div>
            </div>
        </body>
        </html>";

            // Send the email
            await _emailService.SendEmailAsync(Gmail, "Verify Your Email", body);
        }

        public async Task SendResetPasswordEmail(string Gmail)
        {
            // Find the user by email
            var user = await _UserManager.FindByEmailAsync(Gmail);

            // Check if the user exists
            if (user == null)
                return;

            // Generate a verification token
          user.ResetPasswordToken = await _UserManager.GeneratePasswordResetTokenAsync(user);
            user.VerifiedAt = DateTime.UtcNow;

            // Save the token and timestamp to the user
            await _UserManager.UpdateAsync(user);

            // Create a reset URL
            var resetUrl = $"https://localhost:7194/Auth/resetpassword?gmail={Gmail}&token={user.ResetPasswordToken}";

            // Define the email body
            var body = $@"
        <p>Dear {user.UserName},</p>
        <p>You requested a password reset. Please click the link below to reset your password:</p>
        <p><a href='{resetUrl}'>Reset Password</a></p>
        <p>If you did not request this, please ignore this email.</p>
        <p>Best regards,<br>Your Team</p>";

            // Send the email
            await _emailService.SendEmailAsync(Gmail, "Reset Your Password", body);
        }


        public async Task UpdateChatStatus(string ChatId, Chat.ChatStatus Status)
        {
            var FoundChat = await _chatService.GetChatById(ChatId);

            if (FoundChat is null)
                return;

            FoundChat.Status = Status;
            await _DBContext.SaveChangesAsync();
        }

        public async Task<List<Chat>> GetAllChats()
        {
            var chats = await _DBContext.Chats
                .Include(chat => chat.Messages.Where(message => message.AgentAnswered == false))
                .ToListAsync();

            return chats;
        }

        public async Task JoinChat(string ChatId)
        {
            var FoundChat = await _chatService.GetChatById(ChatId);
            if (FoundChat is null)
                return;


            FoundChat.Status = Chat.ChatStatus.Open;

            await _DBContext.SaveChangesAsync();
        }

        public async Task<List<SupportTicket>> GetSupportTickets()
        {
            var SupporTickets = await _DBContext.SupportTickets.Include(x => x.User).ToListAsync();

            return SupporTickets;   
        }

        public async Task<SupportTicket> GetSupportTicketById(string SupportTicketId)
        {
            return await _DBContext.SupportTickets.Include(x => x.User).FirstOrDefaultAsync(x => x.TicketId == SupportTicketId);
        }

        public async Task AddComment(string Comment, string TicketId)
        {
            var LoggedInUser = await _AuthService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return;

            // Check if the logged-in user is an admin
            bool isAdmin = await _UserManager.IsInRoleAsync(LoggedInUser, "Admin");

            

         

            // Create the message first (outside of the loop)
            Message MessageToSend = new Message()
            {
                Content = Comment,
                User = LoggedInUser,
                UserId = LoggedInUser.Id,
                SentAt = DateTime.Now,
                Role = "Admin",
                ConversationType = ConversationType.SupportTicket,
                AgentAnswered = false,
               TicketId = TicketId
               
            };
            // If the logged-in user is an admin, assign the message with AgentId
            if (isAdmin)
            {

                await _DBContext.Messages.AddAsync(MessageToSend);
            }
            else
            {

                MessageToSend.Role = "User";
                await _DBContext.Messages.AddAsync(MessageToSend);
            }


            await _DBContext.SaveChangesAsync();
        }

        public async Task<List<Message>> GetSupportTicketMessages(string TicketId)
        {
            // Check if the user is logged in
            var LoggedInUser = await _AuthService.GetLoggedInUserAsync();

            if (LoggedInUser == null)
            {
                // Return an empty list if no user is logged in
                return new List<Message>();
            }

            // Fetch all messages for the specific ticket, ordered by SentAt
            var TicketMessages = await _DBContext.Messages
                .Include(x => x.User) // Ensure the User info is included with each message
                .Where(x => x.ConversationType == ConversationType.SupportTicket && x.TicketId == TicketId) // Filter by TicketId
                .OrderBy(x => x.SentAt) // Sort messages by the time they were sent
                .ToListAsync(); // Execute the query asynchronously

            // Return the list of messages for the specific ticket
            return TicketMessages;
        }


    }
}
