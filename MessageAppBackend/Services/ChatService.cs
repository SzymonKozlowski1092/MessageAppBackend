using AutoMapper;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public ChatService(MessageAppDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        
        public async Task<Result<ChatDto>> GetChat(Guid chatId)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)!
                .ThenInclude(uc => uc.User)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == chatId && !c.IsDeleted);

            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            if (chat is null)
            {
                return Result.Fail(new Error($"Chat with id {chatId} was not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            if(!chat.Users!.Any(uc => uc.UserId == userId))
            {
                return Result.Fail(new Error($"User with id {getUserIdResult} is not a member of chat with id {chatId}")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }
            var chatDto = _mapper.Map<ChatDto>(chat);

            return Result.Ok(chatDto);
        }

        public async Task<Result<List<MessageDto>>> GetChatMessages(Guid chatId)
        {
            var getCurrentUserIdResult = _currentUserService.GetUserId();
            if (getCurrentUserIdResult.IsFailed)
            {
                return Result.Fail(getCurrentUserIdResult.Errors.First());
            }
            var userId = getCurrentUserIdResult.Value;

            if (_dbContext.UserChats.Any(uc => uc.ChatId == chatId && uc.UserId == userId))
            {
                return Result.Fail(new Error($"User with id {userId} is not a member of chat with id {chatId}")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }

            var messages = await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .ToListAsync();

            var messageDtos = _mapper.Map<List<MessageDto>>(messages);
            return Result.Ok(messageDtos);
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
                ChatId = chat.Id,
                Role = UserChatRole.Admin,
            };

            chat.Users.Add(userChat);
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }

        public async Task<Result> DeleteChat(Guid chatId)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == chatId && !c.IsDeleted);

            var getUserIdResult = _currentUserService.GetUserId();
            if (getUserIdResult.IsFailed)
            {
                return Result.Fail(getUserIdResult.Errors.First());
            }
            var userId = getUserIdResult.Value;

            if (chat is null)
            {
                return Result.Fail(new Error($"Chat with id {chatId} was not found")
                    .WithMetadata("Code", ErrorCode.NotFound));
            }
            if (!await _dbContext.UserChats.AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId))
            {
                return Result.Fail(new Error($"User: {userId} is not a chat: {chatId} member")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }
            if(!await _dbContext.UserChats.AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId && uc.Role == UserChatRole.Admin))
            {
                return Result.Fail(new Error($"User with id: {userId} does not have permission to delete chat with id: {chatId}")
                    .WithMetadata("Code", ErrorCode.Forbidden));
            }

            chat.IsDeleted = true;
            chat.DeletedAt = DateTime.UtcNow;
            
            _dbContext.Chats.Update(chat);
            await _dbContext.SaveChangesAsync();
            
            return Result.Ok();
        }
    }
}
