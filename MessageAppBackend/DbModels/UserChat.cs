using MessageAppBackend.Common.Enums;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace MessageAppBackend.DbModels
{
    public class UserChat
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public UserChatRole Role { get; set; } = UserChatRole.Member;
        [ForeignKey("ChatId")]
        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
