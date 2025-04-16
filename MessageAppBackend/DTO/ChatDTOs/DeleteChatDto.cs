using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.ChatDTOs
{
    public class DeleteChatDto
    {
        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
