using Chat.Application.DTOs;
using Chat.Application.Services;
using Chat.Core.Entities;
using Chat.Infrastructure;
using Chat.Infrastructure.Nats;
using Chat.Web.Hubs;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddRazorPages();

            var idProvider = new CustomUserIdProvider();

            services.AddSingleton<IUserIdProvider>(idProvider);
            services.AddSignalR();

            services.AddCors(o => o.AddPolicy("ChatAppCorsPolicy",
                builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                }));

            services.AddInfrastructure(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("ChatAppCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatSignalr");
            });

            app.UseInfrastructure();

            var bus = app.ApplicationServices.GetRequiredService<INatsBus>();
            var subscription = bus.Subscribe<ChatMessageDto>(message =>
            {
                var hubContext = app.ApplicationServices.GetRequiredService<IHubContext<ChatHub>>();

                hubContext.Clients
                    .User(message.TargetUserName)
                    .SendAsync("SendForReceiveMessage", message);
            }, subjectName: nameof(ChatMessage).Underscore());
        }
    }
}