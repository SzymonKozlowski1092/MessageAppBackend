namespace MessageAppBackend.DTO.ChatInvitationDTOs
{
    public class ChatInvitationDto
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public Guid InvitedUserId { get; set; }
        public string InvitedUsername { get; set; } = string.Empty;
        public Guid InvitedByUserId { get; set; }
        public string InvitedByUsername { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
