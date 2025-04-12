using MessageAppBackend.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MessageAppBackend.Database
{
    public class MessageAppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<ChatInvitation> ChatInvitations { get; set; }

        public MessageAppDbContext(DbContextOptions<MessageAppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region KONFIGURACJA ENCJI

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username)
                .HasMaxLength(25)
                .IsRequired();

                entity.Property(u => u.Email).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.DisplayName)
                .HasMaxLength(30)
                .IsRequired();

                entity.Property(u => u.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(50);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ChatInvitation>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.Property(u => u.InvitedByUserId).IsRequired();
            });
            #endregion

            #region KONFIGURACJA RELACJI

            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId });

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Chats)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.Users)
                .HasForeignKey(uc => uc.ChatId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            modelBuilder.Entity<ChatInvitation>()
                .HasKey(ci => ci.Id);

            modelBuilder.Entity<ChatInvitation>()
                .HasOne(ci => ci.Chat)
                .WithMany(c => c.Invitations)
                .HasForeignKey(ci => ci.ChatId);

            modelBuilder.Entity<ChatInvitation>()
                .HasOne(ci => ci.InvitedUser)
                .WithMany(u => u.ReceivedChatInvitations)
                .HasForeignKey(ci => ci.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatInvitation>()
                .HasOne(ci => ci.InvitedByUser)
                .WithMany(u => u.SentChatInvitations)
                .HasForeignKey(ci => ci.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}