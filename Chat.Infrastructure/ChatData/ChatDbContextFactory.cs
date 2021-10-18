using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatDataContextFactory : DesignTimeDbContextFactoryBase<ChatDataContext>
    {
        protected override ChatDataContext CreateNewInstance(DbContextOptions<ChatDataContext> options)
        {
            return new ChatDataContext(options);
        }
    }
}