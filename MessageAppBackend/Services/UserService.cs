using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class UserService : IUserService
    {
        private readonly MessageAppDbContext _dbContext;
        public UserService(MessageAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Chat>> GetChats(Guid userId)
        {
            var chats = await _dbContext.Chats
                .Where(c => c.Users!
                .Any(uc => uc.UserId == userId))
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .ToListAsync();

            return chats!;
        }
    }
}
