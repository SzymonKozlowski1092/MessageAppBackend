using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetMessage([FromRoute] Guid id) 
        {
            var result = await _messageService.GetMessage(id);
            if(result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid id)
        {
            var result = await _messageService.DeleteMessage(id);
            if (result.IsFailed) 
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] NewMessageDto newMessageDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            
            var result = await _messageService.AddMessage(newMessageDto);
            if(result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessage([FromBody]UpdateMessageDto updateMessageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _messageService.UpdateMessage(updateMessageDto);
            if(result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            
            return NoContent();
        }
    }
}
