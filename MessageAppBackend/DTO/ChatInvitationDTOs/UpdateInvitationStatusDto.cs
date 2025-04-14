using MessageAppBackend.Common.Enums;

namespace MessageAppBackend.DTO.ChatInvitationDTOs
{
    public class UpdateInvitationStatusDto
    {
        public Guid ChatId { get; set; }
        public Guid InvitedUserId { get; set; }
        public InvitationStatus NewStatus { get; set; }
    }
}
