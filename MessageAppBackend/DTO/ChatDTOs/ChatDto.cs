using System.ComponentModel.DataAnnotations;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.UserDTOs;

namespace MessageAppBackend.DTO.ChatDTOs
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<UserDto> Users { get; set; } = new();
        public List<MessageDto> Messages { get; set; } = new();
    }
}
