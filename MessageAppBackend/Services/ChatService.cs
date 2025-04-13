using FluentResults;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _dbContext;
        public ChatService(MessageAppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Message>>> GetMessages(Guid chatId)
        {
            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Where(m => m.ChatId == chatId)
                .ToListAsync();

            if (messages is null || !messages.Any())
            {
                return Result.Fail($"No messages found in chat with id: {chatId}.");
            }

            return Result.Ok(messages);
        }

        public async Task<Result<List<User>>> GetUsers(Guid chatId)
        {
            var users = await _dbContext.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.User)
                .ToListAsync();

            if (users is null || !users.Any())
            {
                Result.Fail($"No users found in chat with id: {chatId}.");
            }

            return Result.Ok(users)!;
        }
    }
}
