using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatInvitationService
    {
        public Task<List<ChatInvitation>> GetUserActiveInvitations(Guid userId);
        public Task<bool> SendChatInvitation(SendInvitationDto sendInvitationDto);
        public Task<bool> AcceptChatInvitation(Guid chatId, Guid invitedUserId);
        public Task<bool> DeclineChatInvitation(Guid chatId, Guid invitedUserId);
    }
}
