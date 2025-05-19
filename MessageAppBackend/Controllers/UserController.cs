using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.UserDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MessageAppBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var result = await _userService.GetUser();
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpGet("/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            var result = await _userService.GetUserByUsername(username);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Ok(result.Value);
        }

        [HttpGet("/chats")]
        public async Task<ActionResult<List<ChatDto>>> GetChats() 
        { 
            var result = await _userService.GetChats();
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpGet("SimpleChats")]
        public async Task<ActionResult<List<SimpleChatDto>>> GetSimpleChats()
        {
            var result = await _userService.GetSimpleChats();
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpDelete("LeaveChat")]
        public async Task<IActionResult> LeaveChat(Guid chatId)
        {
            var result = await _userService.LeaveChat(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            
            return NoContent();
        }
    }
}
