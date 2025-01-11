using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services
{
    public class ChatService : IChatService
    {
        private readonly IAuthService _authService;
        private readonly AppDbContextion _dbcontext;
        public ChatService(IAuthService authService, AppDbContextion dbcontext)
        {
            _authService = authService;
            _dbcontext = dbcontext;
        }

        public async Task<string> CreateChat(string Topic)
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return "";


            var ChatToCreate = new Chat()
            {
                Status = Chat.ChatStatus.Pending,
                ClosedAt = null,
                CreatedAt = DateTime.UtcNow,
                Messages = LoggedInUser.Messages ?? null,
                Topic = Topic,
                User = LoggedInUser,
                UserId = LoggedInUser.Id

            };

            await _dbcontext.AddAsync(ChatToCreate);
            await _dbcontext.SaveChangesAsync();

            return ChatToCreate.ChatId;

        }

        public async Task<Chat> GetChatById(string ChatId)
        {
            var FoundChat = await _dbcontext.Chats.FirstOrDefaultAsync(x => x.ChatId == ChatId);
            return FoundChat;
        }

        public async Task SendChatMessage(string Content)
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null) 
                return;

            var MessageToSend = new Message()
            {
                Content = Content,
              User = LoggedInUser,
              UserId = LoggedInUser.Id,
                SentAt = DateTime.Now
                

            };

            await _dbcontext.AddAsync(MessageToSend);
            await _dbcontext.SaveChangesAsync();    

        }
    }
}
