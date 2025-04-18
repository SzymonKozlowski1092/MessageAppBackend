﻿using MessageAppBackend.Common.Enums;
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ChatInvitationDto>>> GetUserActiveInvitations([FromRoute]Guid userId)
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

        [HttpPut("AcceptInvitation")]
        public async Task<IActionResult> AcceptInvitation([FromBody]UpdateInvitationStatusDto updateInvitationStatusDto)
        {
            var result = await _chatInvitationService
                .AcceptInvitation(updateInvitationStatusDto);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }
        [HttpPut("DeclineInvitation")]
        public async Task<IActionResult> DeclineInvitation([FromBody] UpdateInvitationStatusDto updateInvitationStatusDto)
        {
            var result = await _chatInvitationService
                .DeclineInvitation(updateInvitationStatusDto);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }
    }
}
