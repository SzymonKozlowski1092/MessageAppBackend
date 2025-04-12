using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DbModels
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        public ICollection<UserChat>? Chats { get; set; }

        public ICollection<ChatInvitation>? ReceivedChatInvitations { get; set; }
        public ICollection<ChatInvitation>? SentChatInvitations { get; set; }
    }
}
