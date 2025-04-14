using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IMapper _mapper;
        public MessageService(MessageAppDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    
        public async Task<Result<MessageDto>> GetMessage(Guid id)
        {
            var messageResult = await _dbContext.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);

            return messageResult is null
                ? Result.Fail(new Error($"Message with id: {id} not found")
                    .WithMetadata("Code", ErrorCode.NotFound))
                : Result.Ok(_mapper.Map<MessageDto>(messageResult));
        }
        public async Task<Result> AddMessage(NewMessageDto newMessageDto)
        {
            var newMessage = _mapper.Map<Message>(newMessageDto);
            if(newMessage is null)
            {
                return Result.Fail(new Error("Error with creating new message")
                    .WithMetadata("Code", ErrorCode.FailedOperation));
            }

            await _dbContext.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }
        public async Task<Result> DeleteMessage(Guid id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id)!;
            if(message is null)
            {
                return Result.Fail(new Error("Message to delete not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            _dbContext.Messages.Remove(message);
            
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == updateMessageDto.Id)!;
            if (message is null)
            {
                return Result.Fail(new Error("Message to update not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            message.Content = updateMessageDto.Content;
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
