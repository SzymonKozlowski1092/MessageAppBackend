using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.UserDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ChatService(MessageAppDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Result<List<MessageDto>>> GetMessages(Guid chatId)
        {
            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Where(m => m.ChatId == chatId)
                .ToListAsync();

            if (messages is null || !messages.Any())
            {
                return Result.Fail(new Error($"No messages found in chat with id: {chatId}.")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var messagesDto = _mapper.Map<List<MessageDto>>(messages);
            return Result.Ok(messagesDto);
        }

        public async Task<Result<List<UserDto>>> GetUsers(Guid chatId)
        {
            var users = await _dbContext.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.User)
                .ToListAsync();

            if (users is null || !users.Any())
            {
                return Result.Fail(new Error($"No users found in chat with id: {chatId}.")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }

            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Result.Ok(usersDto)!;
        }

        public async Task<Result> CreateNewChat(CreateChatDto createChatDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == createChatDto.UserId);
            if(user is null)
            {
                return Result.Fail(new Error($"user with id {createChatDto.UserId} for whom you tried to create a chat was not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            var chat = new Chat
            {
                Name = createChatDto.Name,
                CreatedAt = DateTime.UtcNow,
                Users = new List<UserChat>()
            };

            var userChat = new UserChat
            {
                UserId = createChatDto.UserId,
                ChatId = chat.Id
            };

            chat.Users.Add(userChat);
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }

        public async Task<Result> DeleteChat(DeleteChatDto deleteChatDto)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == deleteChatDto.ChatId && !c.IsDeleted);
            if (chat is null)
            {
                return Result.Fail(new Error($"Chat with id {deleteChatDto.ChatId} was not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            if (!await _dbContext.UserChats.AnyAsync(uc => uc.ChatId == deleteChatDto.ChatId && uc.UserId == deleteChatDto.UserId))
            {
                return Result.Fail(new Error($"User: {deleteChatDto.UserId} is not a chat: {deleteChatDto.ChatId} member")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }
           
            //TODO: ADD A USER ROLE IN THE UserChat ENTITY AND CHECK THERE IF THE USER IS THE ADMIN OF THE CHAT
            
            chat.IsDeleted = true;
            chat.DeletedAt = DateTime.UtcNow;
            
            _dbContext.Chats.Update(chat);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }
    }
}
