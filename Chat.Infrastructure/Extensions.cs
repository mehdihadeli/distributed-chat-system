using BlazorChat.Shared;
using Chat.Application.Repositories;
using Chat.Infrastructure.ChatData;
using Chat.Infrastructure.IdentityData;
using Microsoft.AspNetCore.Builder;
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
            services.AddDbContext<ChatDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ChatDataContextConnection")));

            AddCustomIdentity(services, configuration);

            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IIdentityRepository, IdentityRepository>();

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDataContextConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
                .AddEntityFrameworkStores<IdentityDataContext>();

            return services;
        }

        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            IdentitySeeder.SeedSuperAdminAsync(userManager, null).GetAwaiter().GetResult();
        }
    }
}