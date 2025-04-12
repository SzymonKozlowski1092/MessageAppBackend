using MessageAppBackend.Common.Enums;
using MessageAppBackend.Database;
using MessageAppBackend.DTO;
using MessageAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Services
{
    public class ChatInvitationService : IChatInvitationService
    {
        private readonly MessageAppDbContext _dbContext;
        public ChatInvitationService(MessageAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AcceptChatInvitation(Guid chatId, Guid invitedUserId)
        {
            var invitation = await _dbContext.ChatInvitations
                .FirstOrDefaultAsync(
                ci => ci.ChatId == chatId &&
                ci.InvitedUserId == invitedUserId
                && ci.Status == InvitationStatus.Pending);

            if (invitation is null)
            {
                return false;
            }

            invitation.Status = InvitationStatus.Accepted;
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeclineChatInvitation(Guid chatId, Guid invitedUserId)
        {
            var invitation = await _dbContext.ChatInvitations
                .FirstOrDefaultAsync(
                ci => ci.ChatId == chatId && 
                ci.InvitedUserId == invitedUserId 
                && ci.Status == InvitationStatus.Pending);

            if (invitation is null)
            {
                return false;
            }

            invitation.Status = InvitationStatus.Declined;
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public Task<List<ChatInvitation>> GetUserActiveInvitations(Guid userId)
        {
            var invitations = _dbContext.ChatInvitations
                .Include(ci => ci.Chat)
                .Include(ci => ci.InvitedByUser)
                .Where(ci => ci.InvitedUserId == userId && ci.Status == InvitationStatus.Pending)
                .ToListAsync();

            return invitations;
        }
        public async Task<bool> SendChatInvitation(SendInvitationDto sendInvitationDto)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == sendInvitationDto.ChatId);
            
            if(chat is null)
            {
                return false;
            }

            var invitedUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == sendInvitationDto.InvitedUserId);

            if (invitedUser is null)
            {
                return false;
            }

            if (chat.Users!.Any(u => u.UserId == sendInvitationDto.InvitedUserId))
            {
                return false;
            }

            var invitedByUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == sendInvitationDto.InvitedByUserId);

            if (invitedByUser is null)
            {
                return false;
            }

            if(await _dbContext.ChatInvitations.AnyAsync(ci => 
                ci.ChatId == sendInvitationDto.ChatId &&
                ci.InvitedUserId == sendInvitationDto.InvitedUserId &&
                ci.InvitedByUserId == sendInvitationDto.InvitedByUserId))
            {
                return false;
            }

            var chatInvitation = new ChatInvitation
            {
                ChatId = sendInvitationDto.ChatId,
                InvitedUserId = sendInvitationDto.InvitedUserId,
                InvitedByUserId = sendInvitationDto.InvitedByUserId,
                SentAt = DateTime.UtcNow,
                Status = InvitationStatus.Pending
            };
            
            await _dbContext.ChatInvitations.AddAsync(chatInvitation);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
    }
}
