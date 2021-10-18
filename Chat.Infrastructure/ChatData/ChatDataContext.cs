using Chat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatDataContext : DbContext
    {
        public ChatDataContext(DbContextOptions<ChatDataContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }

        public DbSet<ChatConversation> ChatConversation { get; set; }

    }
}
