using CineBook.Interface;
using CineBook.Models;
using CineBook.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class HelpController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IAdminService _adminService;
        private readonly ISupportService _supportService;
        public HelpController(IChatService chatService, IAdminService adminService, ISupportService supportService)
        {
            _chatService = chatService;
            _adminService = adminService;
            _supportService = supportService;
        }

        public IActionResult help()
        {
            return View();
        }

        public IActionResult submitticket()
        {
            return View();
        }

        public async Task<IActionResult> CompactChat(string ChatId)
        {
            string loggedInUserId = User.Identity.Name;

            var chat = await _chatService.GetSameChatMessagesById(ChatId);

            return View(chat);
        }


        public async Task<IActionResult> SendChatMessageAction(string Content, string ChatId)
        {
            var Chat = await _chatService.GetChatById(ChatId);

            if (Chat == null)
            {
                return NotFound("Chat not found");
            }

            await _chatService.SendChatMessage(Content, ChatId);

            return RedirectToAction("CompactChat", "Help", new { Chat.ChatId });
        }

        public async Task<IActionResult> CreateChatAction(string Topic)
        {
            // Create the chat
            var ChatId = await _chatService.CreateChat(Topic);

         
            return RedirectToAction("compactchat", "Help", new { ChatId, Role = "User" });
        }
        public async Task<IActionResult> SubmitASupporticket(string subject, string email, string Category, string message)
        {

            await _supportService.SubmitSupportTicket(subject, email, Category, message);


            return RedirectToAction("Index", "Home");
        }

    }
}
