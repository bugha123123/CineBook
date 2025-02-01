using System.ComponentModel.DataAnnotations;

namespace CineBook.Models
{
    public enum ConversationType
    {
        SupportTicket,
        LiveChat
    }

    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime SentAt { get; set; } 
        public string Content { get; set; } 

        public bool AgentAnswered { get; set; } = false;

        public string? Role { get; set; } = null;

       
        public ConversationType ConversationType { get; set; }


        public string? TicketId { get; set; } = null;
        public string? ChatId { get; set; } = null;
    }
}
