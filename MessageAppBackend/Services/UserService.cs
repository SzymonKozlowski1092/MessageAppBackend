using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class UserService : IUserService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public UserService(MessageAppDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<ChatDto>>> GetChats()
        {
            var getUserIdResult = _currentUserService.GetUserId();
            if(getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            var chats = await _dbContext.Chats
                .Where(c => !c.IsDeleted && c.Users!.Any(uc => uc.UserId == userId))
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .ToListAsync();

            if (chats is null || !chats.Any())
            {
                return Result.Fail(new Error($"No chats found for user with id: {userId}.")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            var chatsDto = _mapper.Map<List<ChatDto>>(chats);

            return Result.Ok(chatsDto);
        }

        public async Task<Result> LeaveChat(Guid chatId)
        {
            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            var userChat = await _dbContext.UserChats
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ChatId == chatId);

            if (userChat is null)
            {
                return Result.Fail(new Error($"No chat found with id: {chatId}, for user with id: {userId}")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            _dbContext.UserChats.Remove(userChat!);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }
    }
}
