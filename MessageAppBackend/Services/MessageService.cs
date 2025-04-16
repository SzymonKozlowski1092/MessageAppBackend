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
            bool chatExists = await _dbContext.Chats.AnyAsync(c => c.Id == newMessageDto.ChatId && !c.IsDeleted);
            bool userExists = await _dbContext.Users.AnyAsync(u => u.Id == newMessageDto.SenderId);

            if (!chatExists || !userExists)
            {
                return Result.Fail(new Error("Unable to add message, chat or user not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            if(!await _dbContext.UserChats.AnyAsync(uc => uc.UserId == newMessageDto.SenderId && uc.ChatId == newMessageDto.ChatId))
            {
                return Result.Fail(new Error("Unable to add message, user is not a member of the chat")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }
            
            var newMessage = _mapper.Map<Message>(newMessageDto);
            if (newMessage is null)
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
            if(!await _dbContext.Messages.AnyAsync(m => m.Id == updateMessageDto.Id && m.SenderId == updateMessageDto.SenderId))
            {
                return Result.Fail(new Error("Unable to update message, user is not the sender")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }

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
