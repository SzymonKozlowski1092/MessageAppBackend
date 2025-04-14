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
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("Messages/{chatId}")]
        public async Task<ActionResult<List<Message>>> GetMessages(Guid chatId)
        {
            var result = await _chatService.GetMessages(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpGet("users/{chatId}")]
        public async Task<ActionResult<List<User>>> GetUsers(Guid chatId)
        {
            var result = await _chatService.GetUsers(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpPost("create/user/{userId}")]
        public async Task<ActionResult<Chat>> CreateNewChat(Guid userId, string chatName)
        {
            var result = await _chatService.CreateNewChat(userId, chatName);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Created();
        }
    }
}
