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
        private readonly ICurrentUserService _currentUserService;
        public MessageService(MessageAppDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _currentUserService = currentUserService;
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
            var getSenderIdResult = _currentUserService.GetUserId();
            if (getSenderIdResult.IsFailed)
            {
                return Result.Fail(getSenderIdResult.Errors.First());
            }
            var senderId = getSenderIdResult.Value;
            
            bool chatExists = await _dbContext.Chats.AnyAsync(c => c.Id == newMessageDto.ChatId && !c.IsDeleted);
            if (!chatExists)
            {
                return Result.Fail(new Error("Unable to add message, chat not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            if(!await _dbContext.UserChats.AnyAsync(uc => uc.UserId == senderId && uc.ChatId == newMessageDto.ChatId))
            {
                return Result.Fail(new Error("Unable to add message, user is not a member of the chat")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }
            
            var newMessage = _mapper.Map<Message>(newMessageDto);
            newMessage.SenderId = senderId;
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

            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            //Check if the user is the sender of the message
            if(!(message.SenderId == userId))
            {
                //Check if the user is admin of the chat
                if(!await _dbContext.UserChats.AnyAsync(uc => 
                uc.UserId == userId &&
                uc.ChatId == message.ChatId &&
                uc.Role == UserChatRole.Admin))
                {
                    return Result.Fail(new Error("Unable to delete message, user is not the sender or chat admin")
                    .WithMetadata("Code", ErrorCode.Forbidden));
                }
            }


            //if (message.SenderId != userId && 
            //    !await _dbContext.UserChats
            //    .AnyAsync(uc => uc.UserId == message.SenderId && 
            //    uc.ChatId == message.ChatId && 
            //    uc.Role == UserChatRole.Admin))
            //{
            //    return Result.Fail(new Error("Unable to delete message, user is not the sender or chat admin")
            //        .WithMetadata("Code", ErrorCode.Forbidden));
            //}

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

            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var senderId = getUserIdResult.Value;

            if (message.SenderId != senderId)
            {
                return Result.Fail(new Error("Unable to update message, user is not the sender")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }

            message.Content = updateMessageDto.Content;
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
