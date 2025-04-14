using MessageAppBackend.DbModels;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.MessageDTOs
{
    public class NewMessageDto
    {
        [Required(ErrorMessage = "Message content cannot be empty.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sender id is required")]
        public Guid SenderId { get; set; }

        [Required(ErrorMessage = "Chat id is required.")]
        public Guid ChatId { get; set; }
    }
}

