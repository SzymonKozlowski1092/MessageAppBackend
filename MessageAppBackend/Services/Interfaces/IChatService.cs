using MessageAppBackend.DbModels;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatService
    {
        public Task<List<Message>> GetMessages(Guid chatId);
        public Task<List<User>> GetUsers(Guid chatId);
    }
}
