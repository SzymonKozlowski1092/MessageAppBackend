using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("messages/{chatId}")]
        public async Task<IActionResult> GetMessages(Guid chatId)
        {
            var messages = await _chatService.GetMessages(chatId);
            if (messages == null)
            {
                return NotFound($"No messages found for chat with id: {chatId}.");
            }
            return Ok(messages);
        }

        [HttpGet("users/{chatId}")]
        public async Task<IActionResult> GetUsers(Guid chatId)
        {
            var users = await _chatService.GetUsers(chatId);
            if (users == null)
            {
                return NotFound($"No users found for chat with id: {chatId}.");
            }
            return Ok(users);
        }
    }
}
