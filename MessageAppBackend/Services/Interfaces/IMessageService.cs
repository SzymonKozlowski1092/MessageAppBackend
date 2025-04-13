using FluentResults;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<Result<Message>> GetMessage(Guid id);
        public Task<Result> DeleteMessage(Guid id);
        public Task<Result> UpdateMessage(Guid id, UpdateMessageDto updateMessageDto);
        public Task<Result<Message>> AddMessage(NewMessageDto newMessageDto);
    }
}
