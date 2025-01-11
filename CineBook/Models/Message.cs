namespace CineBook.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime SentAt { get; set; } // When the message was sent
        public string Content { get; set; } // The actual message content

    }
}
