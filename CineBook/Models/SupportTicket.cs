using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using static CineBook.Models.Chat;

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
        [Key]
        public string TicketId { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; }

        // Status of the ticket: e.g., "Pending", "In Progress", "Resolved",  etc
        public TicketStatus Status { get; set; } = SupportTicket.TicketStatus.Pending;

        public DateTime ClosedAt { get; set; }


        public DateTime? RespondedAt { get; set; }

        // Optionally track if the ticket was resolved or still open
        public bool IsResolved { get; set; } = false;
        public SupportTicket()
        {
            TicketId = GenerateTicketId();
        }

        public string GenerateTicketId()
        {
            Random random = new Random();

            List<string> words = new List<string>
    {
        "Blue", "Sky", "Mountain", "Ocean", "Forest", "Cloud", "River", "Fire", "Leaf", "Stone",
        "Sun", "Star", "Wind", "Dream", "Light", "Shadow", "Thunder", "Flame", "Storm", "Wolf", "Eagle"
    };

            // Pick a random word from the list
            string randomWord = words[random.Next(words.Count)];

            // Generate a random number between 100000 and 999999
            int randomNumber = random.Next(100000, 999999);

            // Combine the random word, random number, and a GUID to ensure uniqueness
            string uniqueId = $"{randomWord}-{randomNumber}-{Guid.NewGuid().ToString().Substring(0, 8)}";

            TicketId = uniqueId;

            return TicketId;
        }

    }
}
