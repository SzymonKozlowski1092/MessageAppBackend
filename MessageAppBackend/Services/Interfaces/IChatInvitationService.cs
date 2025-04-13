using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.DTO;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatInvitationService
    {
        public Task<Result<List<ChatInvitation>>> GetUserActiveInvitations(Guid userId);
        public Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto);
        public Task<Result> UpdateInvitationStatus(Guid chatId, Guid invitedUserId, InvitationStatus newStatus);
    }
}
