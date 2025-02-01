using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services
{
    public class ChatService : IChatService
    {
        private readonly IAuthService _authService;
        private readonly AppDbContextion _dbcontext;
        private readonly UserManager<User> _userManager;
        public ChatService(IAuthService authService, AppDbContextion dbcontext, UserManager<User> userManager)
        {
            _authService = authService;
            _dbcontext = dbcontext;
            _userManager = userManager;
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
            var FoundChat = await _dbcontext.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.ChatId == ChatId);
            return FoundChat;
        }

        public async Task<List<Chat>> GetHistoryOfChat()
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return new List<Chat>();

            var History = await _dbcontext.Chats
                .Where(chat => chat.UserId == LoggedInUser.Id) 
                .Include(chat => chat.Messages) 
                .ToListAsync();

            return History;
        }

     

        public async Task<List<Message>> GetSameChatMessagesById(string ChatId)
        {
            if (string.IsNullOrEmpty(ChatId))
            {
                throw new ArgumentException("ChatId cannot be null or empty", nameof(ChatId));
            }

            var chat = await _dbcontext.Chats
                .Include(chat => chat.Messages)
                .Include(x => x.User)
                .Where(x => x.Messages.Any(x => x.ConversationType == ConversationType.LiveChat))
                .FirstOrDefaultAsync(chat => chat.ChatId == ChatId);

            // If no chat is found, return an empty list of messages
            if (chat == null || chat.Messages == null)
            {
                return new List<Message>();
            }

            return chat.Messages;
        }

        public async Task<List<SupportTicket>> GetSupportTicketMessagesForUser()
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return new List<SupportTicket>();

            return await _dbcontext.SupportTickets.Where(x => x.User.Id == LoggedInUser.Id).ToListAsync();

        }

        public async Task SendChatMessage(string Content, string ChatId)
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return;

            // Check if the logged-in user is an admin
            bool isAdmin = await _userManager.IsInRoleAsync(LoggedInUser, "Admin");

            // Find the chat by ChatId, including its messages
            var Chat = await _dbcontext.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ChatId == ChatId);

            if (Chat == null)
            {
                throw new InvalidOperationException("Chat not found");
            }

            // Create the message first (outside of the loop)
            Message MessageToSend = new Message()
            {
                Content = Content,
                User = LoggedInUser,
                UserId = LoggedInUser.Id,
                SentAt = DateTime.Now,
                Role =  "Admin" ,
                ConversationType = ConversationType.LiveChat,
                ChatId = Chat.ChatId
            };

            // If the logged-in user is an admin, assign the message with AgentId
            if (isAdmin)
            {
                
                Chat.Messages.Add(MessageToSend);
            }
            else
            {
               
                MessageToSend.Role = "User";
                Chat.Messages.Add(MessageToSend);
            }

            // Save changes to the database only once after modifying the collection
            await _dbcontext.SaveChangesAsync();
        }
       


        public async Task<List<Message>> GetLastMessage()
        {
            var LoggedInUser = await _authService.GetLoggedInUserAsync();

            if (LoggedInUser is null)
                return new List<Message>();  

          
            var lastChat = await _dbcontext.Messages
                .Where(x => x.User.Id == LoggedInUser.Id)
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefaultAsync();

            if (lastChat == null)
                return new List<Message>(); 

            
            var chatMessages = await _dbcontext.Messages
                .Where(x => x.ChatId == lastChat.ChatId)
                .ToListAsync();  

           
            if (chatMessages.Any())
            {
               
                var latestMessage = chatMessages.OrderByDescending(x => x.SentAt).FirstOrDefault();

                
                return chatMessages.OrderByDescending(x => x.SentAt).ToList(); 
            }

            return new List<Message>();  
        }

    }
}
