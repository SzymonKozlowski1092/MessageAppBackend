using MessageAppBackend.DbModels;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.MessageDTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderDisplayName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
