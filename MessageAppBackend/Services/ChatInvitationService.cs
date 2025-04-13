using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class ChatInvitationService : IChatInvitationService
    {
        private readonly MessageAppDbContext _dbContext;
        public ChatInvitationService(MessageAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result> UpdateInvitationStatus(Guid chatId, Guid invitedUserId, InvitationStatus newStatus)
        {
            var invitationResult = await GetInvitation(chatId, invitedUserId);
            if (invitationResult.IsFailed)
            {
                return invitationResult.ToResult();
            }
            invitationResult.Value.Status = newStatus;

            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result<List<ChatInvitation>>> GetUserActiveInvitations(Guid userId)
        {
            var invitations = await _dbContext.ChatInvitations
                .Include(ci => ci.Chat)
                .Include(ci => ci.InvitedByUser)
                .Where(ci => ci.InvitedUserId == userId && ci.Status == InvitationStatus.Pending)
                .ToListAsync();

            if (invitations is null || !invitations.Any())
            {
                return Result.Fail($"No invitations found for user with id: {userId}");
            }

            return Result.Ok(invitations);
        }
        public async Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == sendInvitationDto.ChatId);
            if (chat is null)
            {
                return Result.Fail($"Chat with id: {sendInvitationDto.ChatId} not found");
            }

            var invitedUserResult = await GetUser(sendInvitationDto.InvitedUserId, $"Invited user with id {sendInvitationDto.InvitedUserId} not found");
            if (invitedUserResult.IsFailed)
            {
                return invitedUserResult.ToResult();
            }
            var invitedUser = invitedUserResult.Value;

            if (chat.Users!.Any(u => u.UserId == sendInvitationDto.InvitedUserId))
            {
                return Result.Fail($"User with id: {sendInvitationDto.InvitedUserId} already exists in chat with id {sendInvitationDto.ChatId}");
            }

            var invitedByUserResult = await GetUser(sendInvitationDto.InvitedByUserId, $"Invitation sender with id: {sendInvitationDto.InvitedByUserId} not found");
            if (invitedByUserResult.IsFailed)
            {
                return invitedByUserResult.ToResult();
            }
            var invitedByUser = invitedByUserResult.Value;

            if (await _dbContext.ChatInvitations.AnyAsync(ci =>
                ci.ChatId == sendInvitationDto.ChatId &&
                ci.InvitedUserId == sendInvitationDto.InvitedUserId &&
                ci.InvitedByUserId == sendInvitationDto.InvitedByUserId &&
                ci.Status == InvitationStatus.Pending))
            {
                return Result.Fail("This invitation already exists");
            }

            var chatInvitation = new ChatInvitation
            {
                ChatId = sendInvitationDto.ChatId,
                InvitedUserId = sendInvitationDto.InvitedUserId,
                InvitedByUserId = sendInvitationDto.InvitedByUserId,
                SentAt = DateTime.UtcNow,
                Status = InvitationStatus.Pending
            };

            await _dbContext.ChatInvitations.AddAsync(chatInvitation);
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }

        private async Task<Result<User>> GetUser(Guid userId, string notFoundMessage)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null ? Result.Fail(notFoundMessage) : Result.Ok(user);
        }
        private async Task<Result<ChatInvitation>> GetInvitation(Guid chatId, Guid invitedUserId)
        {
            var invitation = await _dbContext.ChatInvitations
                .FirstOrDefaultAsync(
                ci => ci.ChatId == chatId &&
                ci.InvitedUserId == invitedUserId
                && ci.Status == InvitationStatus.Pending);

            return invitation is null ? 
                Result.Fail($"Invitation does not exist for chat with id: {chatId} and user with id: {invitedUserId}") : 
                Result.Ok(invitation);
        }
    }
}
