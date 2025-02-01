using CineBook.Models;

namespace CineBook.Interface
{
    public interface IChatService
    {

        Task SendChatMessage(string Content, string ChatId);

        Task<string> CreateChat(string Topic);

        Task<Chat> GetChatById(string ChatId);

        Task<List<Message>> GetSameChatMessagesById(string ChatId);

        Task<List<Chat>> GetHistoryOfChat();


        Task<List<SupportTicket>> GetSupportTicketMessagesForUser();

        // gets the last message from chat 
        Task<List<Message>> GetLastMessage();
        
    }
}
