using Microsoft.VisualBasic;

namespace MessageAppBackend.DbModels
{
    public class UserChat
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ConversationId { get; set; }
        public Chat? Conversation { get; set; }
    }
}
