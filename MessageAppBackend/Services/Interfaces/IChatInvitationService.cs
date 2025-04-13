using FluentResults;
using MessageAppBackend.DTO;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatInvitationService
    {
        public Task<Result<List<ChatInvitation>>> GetUserActiveInvitations(Guid userId);
        public Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto);
        public Task<Result> AcceptChatInvitation(Guid chatId, Guid invitedUserId);
        public Task<Result> DeclineChatInvitation(Guid chatId, Guid invitedUserId);
    }
}
