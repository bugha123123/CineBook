using Microsoft.IdentityModel.Tokens;

namespace CineBook.Models
{
    public class SupportTicket
    {
        public enum TicketStatus
        {
            Pending,
            InProgress,
            Resolved,
            Closed,
            OnHold
        }
        public string TicketId { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Category { get; set; }

        public DateTime CreatedAt { get; set; }

        // Status of the ticket: e.g., "Pending", "In Progress", "Resolved",  etc
        public TicketStatus Status { get; set; }

        // Optional: Tracking whether the ticket has been flagged as urgent
        public bool IsUrgent { get; set; }

 

        public DateTime? RespondedAt { get; set; }

        // Optionally track if the ticket was resolved or still open
        public bool IsResolved { get; set; }

        public void GenerateTicketId()
        {
            Random random = new Random();
            
            int randomNumber = random.Next(100000, 999999);

            // Take the first 7 characters of the username (if available)
            string usernamePrefix = string.IsNullOrEmpty(User.UserName) ? "Guest" : User.UserName.Substring(0, Math.Min(7, User.UserName.Length));

            // Combine the random number and username prefix to form the TicketId
            TicketId = $"{randomNumber}-{usernamePrefix}";
        }
    }
}
