using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<Message>> GetMessage([FromRoute] Guid id) 
        {
            var message = await _messageService.GetMessage(id);
            if(message is null)
            {
                return NotFound($"Message with ID '{id}' not found.");
            }

            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage([FromRoute] Guid id)
        {
            var result = await _messageService.DeleteMessage(id);
            if (result == false) 
            {
                return NotFound($"Message with ID '{id}' not found.");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> AddMessage([FromBody] NewMessageDto newMessageDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            
            var newMessage = await _messageService.AddMessage(newMessageDto);
            if(newMessage is null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetMessage), new { id = newMessage.Id }, newMessage);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMessage([FromRoute]Guid id, [FromBody]UpdateMessageDto updateMessageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _messageService.UpdateMessage(id, updateMessageDto);
            if(result == false)
            {
                return NotFound($"Message with ID '{updateMessageDto.Id}' not found.");
            }
            
            return NoContent();
        }
    }
}
