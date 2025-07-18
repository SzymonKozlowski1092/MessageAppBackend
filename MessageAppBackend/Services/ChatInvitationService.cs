﻿using AutoMapper;
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
        private readonly ICurrentUserService _currentUserService;
        public ChatInvitationService(MessageAppDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ChatInvitationDto>> GetChatInvitation(Guid invitationId)
        {
            var invitation = await _dbContext.ChatInvitations
                .Include(ci => ci.Chat)
                .Include(ci => ci.InvitedByUser)
                .FirstOrDefaultAsync(i => i.Id == invitationId);

            if (invitation is null)
            {
                return Result.Fail(new Error($"Invitation with id: {invitationId} not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var invitationDto = _mapper.Map<ChatInvitationDto>(invitation);
            return Result.Ok(invitationDto);
        }
        public async Task<Result> AcceptInvitation(Guid invitationId)
        {
            var getInvitedUserIdResult = _currentUserService.GetUserId();
            if (getInvitedUserIdResult.IsFailed)
            {
                return Result.Fail(getInvitedUserIdResult.Errors.First());
            }
            var invitedUserId = getInvitedUserIdResult.Value;

            var invitationResult = await GetInvitation(invitationId, invitedUserId);
            if (invitationResult.IsFailed)
            {
                return invitationResult.ToResult();
            }
            var invitation = invitationResult.Value;

            var userChat = new UserChat
            {
                ChatId = invitationId,
                UserId = invitedUserId
            };
            
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == invitation.ChatId);
            if(chat is null)
            {
                return Result.Fail(new Error($"Error with joining user: {invitedUserId} to chat: {invitationId}. Chat not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            
            chat.Users!.Add(userChat);
            invitation.Status = InvitationStatus.Accepted;
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }        
        public async Task<Result> DeclineInvitation(Guid invitationId)
        {
            var getInvitedUserIdResult = _currentUserService.GetUserId();
            if (getInvitedUserIdResult.IsFailed)
            {
                return Result.Fail(getInvitedUserIdResult.Errors.First());
            }
            var invitedUserId = getInvitedUserIdResult.Value;

            var invitationResult = await GetInvitation(invitationId, invitedUserId);
            if (invitationResult.IsFailed)
            {
                return invitationResult.ToResult();
            }
            var invitation = invitationResult.Value;

            invitation.Status = InvitationStatus.Declined;
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }        
        public async Task<Result<List<ChatInvitationDto>>> GetUserActiveInvitations()
        {
            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

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
            var getInvitedByUserIdResult = _currentUserService.GetUserId();
            if (getInvitedByUserIdResult.IsFailed)
            {
                return Result.Fail(getInvitedByUserIdResult.Errors.First());
            }
            var invitedByUserId = getInvitedByUserIdResult.Value;

            if (await _dbContext.ChatInvitations.AnyAsync(ci =>
                ci.ChatId == sendInvitationDto.ChatId &&
                ci.InvitedUserId == sendInvitationDto.InvitedUserId &&
                ci.InvitedByUserId == invitedByUserId &&
                ci.Status == InvitationStatus.Pending))
            {
                return Result.Fail(new Error("This invitation already exists")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
            }

            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == sendInvitationDto.ChatId && !c.IsDeleted);
            if (chat is null)
            {
                return Result.Fail(new Error($"Chat with id: {sendInvitationDto.ChatId} not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            if (chat.Users!.Any(u => u.UserId == sendInvitationDto.InvitedUserId))
            {
                return Result.Fail(new Error($"User with id: {sendInvitationDto.InvitedUserId} already exists in chat with id {sendInvitationDto.ChatId}")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
            }

            var invitedUserResult = await GetUser(sendInvitationDto.InvitedUserId, $"Invited user with id {sendInvitationDto.InvitedUserId} not found");
            if (invitedUserResult.IsFailed)
            {
                return invitedUserResult.ToResult();
            }
            var invitedUser = invitedUserResult.Value;

            var chatInvitation = new ChatInvitation
            {
                ChatId = sendInvitationDto.ChatId,
                InvitedUserId = sendInvitationDto.InvitedUserId,
                InvitedByUserId = invitedByUserId,
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
        private async Task<Result<ChatInvitation>> GetInvitation(Guid invitationId, Guid invitedUserId)
        {
            var invitation = await _dbContext.ChatInvitations
                .Include(ci => ci.Chat)
                .FirstOrDefaultAsync(
                ci => ci.Id == invitationId &&
                !ci.Chat.IsDeleted &&
                ci.InvitedUserId == invitedUserId &&
                ci.Status == InvitationStatus.Pending);

            return invitation is null ? 
                Result.Fail(new Error($"Invitation with id: {invitationId} does not exist for user with id: {invitedUserId}")
                .WithMetadata("Code", ErrorCode.NotFound)) : 
                Result.Ok(invitation);
        }
    }
}
