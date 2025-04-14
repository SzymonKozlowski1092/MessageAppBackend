using MessageAppBackend.DbModels;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.ChatDTOs
{
    public class CreateChatDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
