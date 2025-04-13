using FluentResults;
using MessageAppBackend.DbModels;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<List<Chat>>> GetChats(Guid userId);
        public Task<Result> LeaveChat(Guid userId, Guid chatId);
    }
}
