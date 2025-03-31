using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageAppBackend.DbModels
{
    public class UserChat
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        [ForeignKey("ChatId")]
        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
