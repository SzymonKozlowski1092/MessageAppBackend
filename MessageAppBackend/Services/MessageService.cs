using AutoMapper;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
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

        public async Task<Message> AddMessage(NewMessageDto newMessageDto)
        {
            var newMessage = _mapper.Map<Message>(newMessageDto);
            if(newMessage is null)
            {
                return null!;
            }

            await _dbContext.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            
            return newMessage;
        }

        public async Task<bool> DeleteMessage(Guid id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if(message is null)
            {
                return false;
            }

            _dbContext.Messages.Remove(message);
            _dbContext.SaveChanges();
            return true;
        }

        public async Task<Message> GetMessage(Guid id)
        {
            var message = await _dbContext.Messages.
                Include(m => m.Sender).
                FirstOrDefaultAsync(m => m.Id == id);
            
            if (message is null)
            {
                return null!;
            }

            return message;
        }

        public async Task<bool> UpdateMessage(Guid id, UpdateMessageDto updateMessageDto)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message is null)
            {
                return false;
            }

            message.Content = updateMessageDto.Content;
            _dbContext.SaveChanges();

            return true;
        }
    }
}
