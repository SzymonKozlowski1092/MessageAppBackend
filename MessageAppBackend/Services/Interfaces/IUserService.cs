using FluentResults;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<List<ChatDto>>> GetChats();
        public Task<Result> LeaveChat(Guid chatId);
    }
}
