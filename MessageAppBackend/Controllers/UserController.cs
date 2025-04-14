using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DbModels;
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

        [HttpGet("/{userId}/chats")]
        public async Task<ActionResult<List<Chat>>> GetChats(Guid userId) 
        { 
            var result = await _userService.GetChats(userId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpDelete("{userId}/leave/{chatId}")]
        public async Task<IActionResult> LeaveChat(Guid userId, Guid chatId)
        {
            var result = await _userService.LeaveChat(userId, chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            
            return NoContent();
        }
    }
}
