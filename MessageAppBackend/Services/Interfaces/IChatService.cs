using FluentResults;
using MessageAppBackend.DbModels;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatService
    {
        public Task<Result<List<Message>>> GetMessages(Guid chatId);
        public Task<Result<List<User>>> GetUsers(Guid chatId);
        public Task<Result<Chat>> CreateNewChat(Guid userId, string chatName);
    }
}
