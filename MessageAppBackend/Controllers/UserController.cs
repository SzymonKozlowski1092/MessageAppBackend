using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
    }
}
