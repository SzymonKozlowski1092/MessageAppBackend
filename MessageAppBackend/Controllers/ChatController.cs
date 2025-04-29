using System.Security.Claims;
using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.UserDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("{chatId}")]
        public async Task<ActionResult<List<ChatDto>>> GetChat(Guid chatId)
        {
            var result = await _chatService.GetChat(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpGet("{chatId}/Messages")]
        public async Task<ActionResult<List<MessageDto>>> GetChatMessages(Guid chatId)
        {
            var result = await _chatService.GetChatMessages(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewChat([FromBody] CreateChatDto createChatDto)
        {
            var result = await _chatService.CreateNewChat(createChatDto);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Created();
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat([FromRoute]Guid chatId)
        {
            var result = await _chatService.DeleteChat(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return NoContent();
        }
    }
}
