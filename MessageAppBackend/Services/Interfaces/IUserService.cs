using MessageAppBackend.DbModels;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<Chat>> GetChats(Guid userId);

    }
}
