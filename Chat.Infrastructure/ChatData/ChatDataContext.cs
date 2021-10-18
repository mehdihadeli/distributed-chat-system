using BlazorChat.Shared;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatDataContext : DbContext
    {
        public ChatDataContext(DbContextOptions<ChatDataContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Message);

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.ChatMessagesFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatMessagesToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
