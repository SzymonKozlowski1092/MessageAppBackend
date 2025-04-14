using MessageAppBackend.Common.Enums;
using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatInvitationController : ControllerBase
    {
        private readonly IChatInvitationService _chatInvitationService;
        public ChatInvitationController(IChatInvitationService chatInvitationService)
        {
            _chatInvitationService = chatInvitationService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ChatInvitation>>> GetUserActiveInvitations(Guid userId)
        {
            var result = await _chatInvitationService.GetUserActiveInvitations(userId);
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

        [HttpPut]
        public async Task<IActionResult> UpdateInvitationStatus(Guid chatId, Guid invitedUserId, InvitationStatus newStatus)
        {
            var result = await _chatInvitationService.UpdateInvitationStatus(chatId, invitedUserId, newStatus);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }
    }
}
