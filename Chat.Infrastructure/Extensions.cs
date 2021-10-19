using Chat.Application.Repositories;
using Chat.Application.Services;
using Chat.Core.Entities;
using Chat.Infrastructure.ChatData;
using Chat.Infrastructure.Data;
using Chat.Infrastructure.IdentityData;
using Chat.Infrastructure.Nats;
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
            AddCustomIdentity(services, configuration);

            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IIdentityRepository, IdentityRepository>();
            services.AddSingleton<INatsBus, NatsBus>();

            services.AddOptions<NatsOptions>().Bind(configuration.GetSection("NatsOptions"))
                .ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ChatDataContextConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
                .AddEntityFrameworkStores<ApplicationDataContext>();

            return services;
        }

        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            SeedIdentity(app);
        }

        private static void SeedIdentity(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            IdentitySeeder.SeedSuperAdminAsync(userManager, null).GetAwaiter().GetResult();
        }
    }
}