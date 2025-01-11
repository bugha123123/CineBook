using System;
using System.Collections.Generic;

namespace CineBook.Models
{
    public class Chat
    {

        public enum ChatStatus
        {
            Open,    // Chat is currently active
            Pending, // Chat is waiting for user or agent action
            Closed   // Chat has been resolved or ended
        }
        public string ChatId { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? ClosedAt { get; set; } // When the chat was closed, if applicable

        public string UserId { get; set; } 
        public User User { get; set; }

        public ChatStatus Status { get; set; }
        public string Topic { get; set; } 

        public List<Message> Messages { get; set; } // Collection of messages in the chat

        public Chat()
        {
            ChatId = GenerateChatId();
            
            Status = ChatStatus.Closed; 
            Messages = new List<Message>();
        }

        private string GenerateChatId()
        {
            // Generate a unique identifier similar to a support ticket
            string prefix = "#";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string randomDigits = new Random().Next(1000, 4999).ToString(); 
            return $"{prefix}{timestamp}{randomDigits}";
        }
    }


  
}
