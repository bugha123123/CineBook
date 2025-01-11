using CineBook.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class HelpController : Controller
    {
        private readonly IChatService _chatService;

        public HelpController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult help()
        {
            return View();
        }

        public IActionResult submitticket()
        {
            return View();
        }

        public IActionResult compactchat()
        {
            return View();
        }

        public async Task<IActionResult> SendChatMessageAction(string Content)
        {
           

            await _chatService.SendChatMessage(Content);
            return RedirectToAction("compactchat", "Help");


        }

        public async Task<IActionResult> CreateChatAction(string Topic)
        {
            // Create the chat
            var ChatId = await _chatService.CreateChat(Topic);

         
            return RedirectToAction("compactchat", "Help", new { ChatId });
        }

    }
}
