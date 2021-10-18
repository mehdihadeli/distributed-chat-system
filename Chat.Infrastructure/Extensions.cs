using Chat.Infrastructure.ChatData;
using Chat.Infrastructure.IdentityData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDataContextConnection")));

            services.AddDbContext<ChatDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ChatDataContextConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
                .AddEntityFrameworkStores<IdentityDataContext>();

            return services;
        }
    }
}