using FluentResults;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class UserService : IUserService
    {
        private readonly MessageAppDbContext _dbContext;
        public UserService(MessageAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Chat>>> GetChats(Guid userId)
        {
            var chats = await _dbContext.Chats
                .Where(c => c.Users!
                .Any(uc => uc.UserId == userId))
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .ToListAsync();

            if (chats.IsNullOrEmpty())
            {
                return Result.Fail($"No chats found for user with id: {userId}.");
            }

            return Result.Ok(chats);
        }

        public async Task<Result> LeaveChat(Guid userId, Guid chatId)
        {
            var userChat = await _dbContext.UserChats
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ChatId == chatId);

            if (userChat is null)
            {
                return Result.Fail($"No chat found with id: {chatId}, for user with id: {userId}");
            }

            _dbContext.UserChats.Remove(userChat!);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
