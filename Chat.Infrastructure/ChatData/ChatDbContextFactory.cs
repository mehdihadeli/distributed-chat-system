using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatDbContextFactory : DesignTimeDbContextFactoryBase<ChatDbContext>
    {
        protected override ChatDbContext CreateNewInstance(DbContextOptions<ChatDbContext> options)
        {
            return new ChatDbContext(options);
        }
    }
}