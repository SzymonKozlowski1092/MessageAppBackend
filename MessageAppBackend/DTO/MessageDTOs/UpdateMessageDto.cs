using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.MessageDTOs
{
    public class UpdateMessageDto
    {
        [Required(ErrorMessage = "Message content cannot be empty")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
