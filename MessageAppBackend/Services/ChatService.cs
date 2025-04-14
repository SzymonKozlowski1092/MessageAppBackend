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

        public async Task<Result<ChatDto>> CreateNewChat(CreateChatDto createChatDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == createChatDto.UserId);
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
            
            var chatDto = _mapper.Map<ChatDto>(chat);
            return Result.Ok(chatDto);
        }
    }
}
