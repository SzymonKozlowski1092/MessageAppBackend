using MessageAppBackend.Common.Enums;

namespace MessageAppBackend.DTO.ChatInvitationDTOs
{
    public class ChatInvitationDto
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public string InvitedByUsername { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        //public InvitationStatus Status { get; set; }
    }
}
