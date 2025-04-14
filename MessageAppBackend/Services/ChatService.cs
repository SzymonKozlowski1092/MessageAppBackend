﻿using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly MessageAppDbContext _dbContext;
        public ChatService(MessageAppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Message>>> GetMessages(Guid chatId)
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

            return Result.Ok(messages);
        }

        public async Task<Result<List<User>>> GetUsers(Guid chatId)
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

            return Result.Ok(users)!;
        }
    }
}
