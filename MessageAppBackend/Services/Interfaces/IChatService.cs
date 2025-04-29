using FluentResults;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.UserDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IChatService
    {
        public Task<Result<ChatDto>> GetChat(Guid chatId);
        public Task<Result<List<MessageDto>>> GetChatMessages(Guid chatId);
        public Task<Result> CreateNewChat(CreateChatDto createChatDto);
        public Task<Result> DeleteChat(Guid chatId);
    }
}
