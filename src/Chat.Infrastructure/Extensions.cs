using System;
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

            services.AddTransient<IChatRepository, EFChatRepository>();
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

            services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    //https://www.yogihosting.com/aspnet-core-identity-user-lockout/
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddEntityFrameworkStores<ApplicationDataContext>();

            return services;
        }

        public static void UseInfrastructure(this IApplicationBuilder app)
        {
             MigrateDatabase(app);
             SeedIdentity(app);
        }

        private static void MigrateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
            context.Database.Migrate();
        }

        private static void SeedIdentity(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            IdentitySeeder.SeedSuperAdminAsync(userManager, null).GetAwaiter().GetResult();
        }
    }
}