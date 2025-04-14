using FluentResults;
using MessageAppBackend.DTO.ChatInvitationDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatInvitationService
    {
        public Task<Result<List<ChatInvitationDto>>> GetUserActiveInvitations(Guid userId);
        public Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto);
        public Task<Result> UpdateInvitationStatus(UpdateInvitationStatusDto updateInvitationStatusDto);
    }
}
