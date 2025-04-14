using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatInvitationDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class ChatInvitationService : IChatInvitationService
    {
        private readonly IMapper _mapper;
        private readonly MessageAppDbContext _dbContext;
        public ChatInvitationService(MessageAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<Result> UpdateInvitationStatus(UpdateInvitationStatusDto updateInvitationStatusDto)
        {
            var invitationResult = await GetInvitation(updateInvitationStatusDto.ChatId, updateInvitationStatusDto.InvitedUserId);
            if (invitationResult.IsFailed)
            {
                return invitationResult.ToResult();
            }
            invitationResult.Value.Status = updateInvitationStatusDto.NewStatus;

            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result<List<ChatInvitationDto>>> GetUserActiveInvitations(Guid userId)
        {
            var invitations = await _dbContext.ChatInvitations
                .Include(ci => ci.Chat)
                .Include(ci => ci.InvitedByUser)
                .Where(ci => ci.InvitedUserId == userId && ci.Status == InvitationStatus.Pending)
                .ToListAsync();

            if (invitations is null || !invitations.Any())
            {
                return Result.Fail(new Error($"No invitations found for user with id: {userId}")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var invitationsDto = _mapper.Map<List<ChatInvitationDto>>(invitations);
            return Result.Ok(invitationsDto);
        }
        public async Task<Result> SendChatInvitation(SendInvitationDto sendInvitationDto)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == sendInvitationDto.ChatId);
            if (chat is null)
            {
                return Result.Fail(new Error($"Chat with id: {sendInvitationDto.ChatId} not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var invitedUserResult = await GetUser(sendInvitationDto.InvitedUserId, $"Invited user with id {sendInvitationDto.InvitedUserId} not found");
            if (invitedUserResult.IsFailed)
            {
                return invitedUserResult.ToResult();
            }
            var invitedUser = invitedUserResult.Value;

            if (chat.Users!.Any(u => u.UserId == sendInvitationDto.InvitedUserId))
            {
                return Result.Fail(new Error($"User with id: {sendInvitationDto.InvitedUserId} already exists in chat with id {sendInvitationDto.ChatId}")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
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
                return Result.Fail(new Error("This invitation already exists")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
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

            return user is null ? 
                Result.Fail(new Error(notFoundMessage).WithMetadata("Code", ErrorCode.NotFound)) :
                Result.Ok(user);
        }
        private async Task<Result<ChatInvitation>> GetInvitation(Guid chatId, Guid invitedUserId)
        {
            var invitation = await _dbContext.ChatInvitations
                .FirstOrDefaultAsync(
                ci => ci.ChatId == chatId &&
                ci.InvitedUserId == invitedUserId
                && ci.Status == InvitationStatus.Pending);

            return invitation is null ? 
                Result.Fail(new Error($"Invitation does not exist for chat with id: {chatId} and user with id: {invitedUserId}").WithMetadata("Code", ErrorCode.NotFound)) : 
                Result.Ok(invitation);
        }
    }
}
