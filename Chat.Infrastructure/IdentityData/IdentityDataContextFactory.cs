using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.IdentityData
{
    public class IdentityDataContextFactory : DesignTimeDbContextFactoryBase<IdentityDataContext>
    {
        protected override IdentityDataContext CreateNewInstance(DbContextOptions<IdentityDataContext> options)
        {
            return new IdentityDataContext(options);
        }
    }
}