using MessageAppBackend.Common.Enums;
using MessageAppBackend.DbModels;
public class ChatInvitation
{
    public Guid Id { get; set; }

    public Guid ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public Guid InvitedUserId { get; set; }
    public User InvitedUser { get; set; } = null!;

    public Guid InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
