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
        public UserService(MessageAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Result<List<ChatDto>>> GetChats(Guid userId)
        {
            var chats = await _dbContext.Chats
                .Where(c => c.Users!
                .Any(uc => uc.UserId == userId))
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

        public async Task<Result> LeaveChat(LeaveChatDto leaveChatDto)
        {
            var userChat = await _dbContext.UserChats
                .FirstOrDefaultAsync(uc => uc.UserId == leaveChatDto.UserId && uc.ChatId == leaveChatDto.ChatId);

            if (userChat is null)
            {
                return Result.Fail(new Error($"No chat found with id: {leaveChatDto.ChatId}, for user with id: {leaveChatDto.UserId}")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            _dbContext.UserChats.Remove(userChat!);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
