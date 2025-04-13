using AutoMapper;
using FluentResults;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
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
    
        public async Task<Result<Message>> GetMessage(Guid id)
        {
            var messageResult = await _dbContext.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);

            return messageResult is null
                ? Result.Fail($"Message with id: {id} not found")
                : Result.Ok(messageResult);
        }
        public async Task<Result<Message>> AddMessage(NewMessageDto newMessageDto)
        {
            var newMessage = _mapper.Map<Message>(newMessageDto);
            if(newMessage is null)
            {
                return Result.Fail("Error with creating new message");
            }

            await _dbContext.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok(newMessage);
        }
        public async Task<Result> DeleteMessage(Guid id)
        {
            var messageResult = await GetMessage(id);
            if(messageResult.IsFailed)
            {
                return messageResult.ToResult();
            }
            var message = messageResult.Value;

            _dbContext.Messages.Remove(message);
            _dbContext.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> UpdateMessage(Guid id, UpdateMessageDto updateMessageDto)
        {
            var messageResult = await GetMessage(id);
            if (messageResult.IsFailed)
            {
                return messageResult.ToResult();
            }
            var message = messageResult.Value;

            message.Content = updateMessageDto.Content;
            _dbContext.SaveChanges();

            return Result.Ok();
        }
    }
}
