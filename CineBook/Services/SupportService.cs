using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;

namespace CineBook.Services
{
    public class SupportService : ISupportService
    {

        private readonly IAuthService _authService;
        private readonly AppDbContextion _dbContext;

        public SupportService(IAuthService authService, AppDbContextion dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        public async Task SubmitSupportTicket(string subject, string email, string category, string message)
        {
            // Get the logged-in user
            var loggedInUser = await _authService.GetLoggedInUserAsync();

            if (loggedInUser is null)
                return;

            // Create a new support ticket
            var ticketToAdd = new SupportTicket()
            {
                Status = SupportTicket.TicketStatus.Pending,
                Subject = subject,
                Category = category,
                CreatedAt = DateTime.UtcNow,
                IsResolved = false,
                Message = message,
                RespondedAt = null,
                User = loggedInUser,
                UserId = loggedInUser.Id,
            };



            // Add the support ticket to the database and save changes
            await _dbContext.AddAsync(ticketToAdd);
            await _dbContext.SaveChangesAsync();
        }

    }
}
