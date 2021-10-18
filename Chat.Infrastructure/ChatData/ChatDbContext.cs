using Chat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }

        public DbSet<ChatConversation> ChatConversation { get; set; }

    }
}
