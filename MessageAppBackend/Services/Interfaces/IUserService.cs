using FluentResults;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.UserDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> GetUser();
        public Task<Result<UserDto>> GetUserByUsername(string username);
        public Task<Result<List<ChatDto>>> GetChats();
        public Task<Result<List<SimpleChatDto>>> GetSimpleChats();
        public Task<Result> LeaveChat(Guid chatId);
    }
}
