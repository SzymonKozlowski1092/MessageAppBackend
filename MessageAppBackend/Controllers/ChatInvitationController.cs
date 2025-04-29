using MessageAppBackend.Common.Enums;
using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO.ChatInvitationDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatInvitationController : ControllerBase
    {
        private readonly IChatInvitationService _chatInvitationService;
        public ChatInvitationController(IChatInvitationService chatInvitationService)
        {
            _chatInvitationService = chatInvitationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatInvitationDto>>> GetUserActiveInvitations()
        {
            var result = await _chatInvitationService.GetUserActiveInvitations();
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> SendChatInvitation([FromBody] SendInvitationDto sendInvitationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _chatInvitationService.SendChatInvitation(sendInvitationDto);
            if(result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Created();
        }

        [HttpPut("AcceptInvitation/{chatId}")]
        public async Task<IActionResult> AcceptInvitation(Guid chatId)
        {
            var result = await _chatInvitationService
                .AcceptInvitation(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }
        [HttpPut("DeclineInvitation/{chatId}")]
        public async Task<IActionResult> DeclineInvitation(Guid chatId)
        {
            var result = await _chatInvitationService
                .DeclineInvitation(chatId);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }
    }
}
