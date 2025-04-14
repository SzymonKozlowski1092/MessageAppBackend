using FluentResults;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.UserDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatService
    {
        public Task<Result<List<MessageDto>>> GetMessages(Guid chatId);
        public Task<Result<List<UserDto>>> GetUsers(Guid chatId);
        public Task<Result<ChatDto>> CreateNewChat(CreateChatDto createChatDto);
    }
}
