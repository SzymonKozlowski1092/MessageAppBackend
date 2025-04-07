using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _dbContext;
        public ChatService(MessageAppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<Message>> GetMessages(Guid chatId)
        {
            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Where(m => m.ChatId == chatId)
                .ToListAsync();

            return messages!;
        }

        public async Task<List<User>> GetUsers(Guid chatId)
        {
            var users = await _dbContext.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.User)
                .ToListAsync();

            return users!;
        }
    }
}
