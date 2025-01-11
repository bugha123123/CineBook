using CineBook.Models;

namespace CineBook.Interface
{
    public interface IChatService
    {

        Task SendChatMessage(string Content);

        Task<string> CreateChat(string Topic);

        Task<Chat> GetChatById(string ChatId);
    }
}
