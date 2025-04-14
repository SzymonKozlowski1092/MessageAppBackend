using FluentResults;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.MessageDTOs;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<Result<MessageDto>> GetMessage(Guid id);
        public Task<Result> DeleteMessage(Guid id);
        public Task<Result> UpdateMessage(UpdateMessageDto updateMessageDto);
        public Task<Result> AddMessage(NewMessageDto newMessageDto);
    }
}
