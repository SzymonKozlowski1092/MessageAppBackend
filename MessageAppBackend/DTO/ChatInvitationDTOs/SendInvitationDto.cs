using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO.ChatInvitationDTOs
{
    public class SendInvitationDto
    {
        [Required(ErrorMessage = "Chat id is required")]
        public Guid ChatId { get; set; }
        [Required(ErrorMessage = "Invited user id is required")]
        public Guid InvitedUserId { get; set; }
        [Required(ErrorMessage = "Sender id is required")]
        public Guid InvitedByUserId { get; set; }
    }
}
