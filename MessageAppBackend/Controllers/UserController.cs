using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO.ChatDTOs;
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

        [HttpGet("/{UserId}/chats")]
        public async Task<ActionResult<List<ChatDto>>> GetChats(Guid userId) 
        { 
            var result = await _userService.GetChats(userId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpDelete("{UserId}/leave/{chatId}")]
        public async Task<IActionResult> LeaveChat(LeaveChatDto leaveChatDto)
        {
            var result = await _userService.LeaveChat(leaveChatDto);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            
            return NoContent();
        }
    }
}
