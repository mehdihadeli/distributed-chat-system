using Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.IdentityData
{
    public class ApplicationDataContextFactory : DesignTimeDbContextFactoryBase<ApplicationDataContext>
    {
        protected override ApplicationDataContext CreateNewInstance(DbContextOptions<ApplicationDataContext> options)
        {
            return new ApplicationDataContext(options);
        }
    }
}