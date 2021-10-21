using Chat.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
           return services.AddTransient<IChatService, ChatService>();
        }
    }
}