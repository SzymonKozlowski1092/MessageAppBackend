using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("chats/{userId}")]
        public async Task<IActionResult> GetChats(Guid userId)
        {
            var chats = await _userService.GetChats(userId);
            if (chats == null || !chats.Any())
            {
                return NotFound($"No chats found for user with id: {userId}.");
            }
            return Ok(chats);
        }

        [HttpDelete("{userId}/leave/{chatId}")]
        public async Task<IActionResult> LeaveChat(Guid userId, Guid chatId)
        {
            var result = await _userService.LeaveChat(userId, chatId);
            if (result == false)
            {
                return NotFound($"User with id: {userId} is not a member of chat with id: {chatId}.");
            }
            
            return NoContent();
        }
    }
}
