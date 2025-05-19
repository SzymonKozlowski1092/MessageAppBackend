using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.UserDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<UserDto>> GetUser()
        {
            var getCurrentUserIdResult = _currentUserService.GetUserId();
            if (getCurrentUserIdResult.IsFailed)
            {
                return Result.Fail(getCurrentUserIdResult.Errors.First());
            }
            var userId = getCurrentUserIdResult.Value;

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return Result.Fail(new Error($"No user found with id: {userId}.")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Ok(userDto);
        }

        public async Task<Result<UserDto>> GetUserByUsername(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null) 
            {
                return Result.Fail(new Error($"User with the username: {username} not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Ok(userDto);
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

        public async Task<Result<List<SimpleChatDto>>> GetSimpleChats()
        {
            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            var chats = await _dbContext.Chats
                .Where(c => !c.IsDeleted && c.Users!.Any(uc => uc.UserId == userId))
                .Include(c => c.Messages)!
                .ThenInclude(m => m.Sender)
                .ToListAsync();

            if (!chats.Any())
            {
                return Result.Fail(new Error($"No chats found for user with id: {userId}.")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var simpleChatsDto = _mapper.Map<List<SimpleChatDto>>(chats);
            return Result.Ok(simpleChatsDto);
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
