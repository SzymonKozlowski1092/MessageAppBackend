using FluentResults;
using MessageAppBackend.DTO.ChatInvitationDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatInvitationService
    {
        public Task<Result<List<ChatInvitationDto>>> GetUserActiveInvitations();
        public Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto);
        public Task<Result> AcceptInvitation(Guid chatId);
        public Task<Result> DeclineInvitation(Guid chatId);
    }
}
