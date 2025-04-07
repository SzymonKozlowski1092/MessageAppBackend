using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<Message> GetMessage(Guid id);
        public Task<bool> DeleteMessage(Guid id);
        public Task<bool> UpdateMessage(Guid id, UpdateMessageDto updateMessageDto);
        public Task<Message> AddMessage(NewMessageDto newMessageDto);
    }
}
