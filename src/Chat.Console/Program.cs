using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Chat.Application.DTOs;
using Chat.Application.Services;
using Chat.Core.Entities;
using Chat.Infrastructure.Nats;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chat.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var host = CreateHostBuilder(args, config).Build();
            var services = host.Services;


            System.Console.WriteLine("Please enter your user name:");
            var userName = System.Console.ReadLine()?.Trim();
            System.Console.WriteLine("Please enter your target user name for chatting:");
            var targetUserName = System.Console.ReadLine()?.Trim();
            SubscribeOnChatMessage(userName, services);
            await LoadPreviouslyReceivedMessages(userName, services);

            System.Console.WriteLine("Please enter message:");
            while (true)
            {
                var inputMessage = System.Console.ReadLine();
                if (inputMessage.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                await SendMessage(userName, targetUserName, inputMessage, services);
            }
        }

        private static async Task LoadPreviouslyReceivedMessages(string userName, IServiceProvider services)
        {
            var clientFactory = services.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("WebAPIClient");
            var numberOfLastMessages = 50;
            var result =
                await client.GetFromJsonAsync<IEnumerable<ChatMessageDto>>
                    ($"api/chat/load-messages/{userName}?numberOfMessages={numberOfLastMessages}");

            if (result != null)
            {
                foreach (var chatMessageDto in result)
                {
                    if (chatMessageDto.SenderUserName == userName)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine(
                            $"-> {chatMessageDto.MessageDate} - {chatMessageDto.SenderUserName} to {chatMessageDto.TargetUserName}: {chatMessageDto.Message}");
                        System.Console.ResetColor();
                    }
                    else
                    {
                        System.Console.ForegroundColor = ConsoleColor.Cyan;
                        System.Console.WriteLine(
                            $"<- {chatMessageDto.MessageDate} - {chatMessageDto.SenderUserName}: {chatMessageDto.Message}");
                        System.Console.ResetColor();
                    }
                }
            }
        }

        private static void SubscribeOnChatMessage(string userName, IServiceProvider services)
        {
            var bus = services.GetRequiredService<INatsBus>();
            var subscription = bus.Subscribe<ChatMessageDto>(
                chatMessage =>
                {
                    if (chatMessage.TargetUserName == userName)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Cyan;
                        System.Console.WriteLine(
                            $"<- {chatMessage.MessageDate} - {chatMessage.SenderUserName}: {chatMessage.Message}");
                        System.Console.ResetColor();
                    }
                }, nameof(ChatMessage).Underscore());
        }

        private static async Task SendMessage(string userName, string targetUserName, string message,
            IServiceProvider services)
        {
            var clientFactory = services.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("WebAPIClient");
            var sendMessageDto = new SendMessageDto
            {
                Message = message,
                SenderUserName = userName,
                TargetUserName = targetUserName
            };

            var response = await client.PostAsJsonAsync("api/chat/send-message", sendMessageDto);





            
            response.EnsureSuccessStatusCode();

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(
                $"-> {DateTime.Now} - {sendMessageDto.SenderUserName} to {sendMessageDto.TargetUserName}: {sendMessageDto.Message}");
            System.Console.ResetColor();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(config => { config.ClearProviders(); })
                .ConfigureServices((_, services) =>
                    {
                        services.AddTransient<INatsBus, NatsBus>()
                            .AddOptions<NatsOptions>().Bind(configuration.GetSection("NatsOptions"))
                            .ValidateDataAnnotations();

                        var apiAddress = configuration.GetSection("APIAddress").Get<string>();

                        services.AddHttpClient("WebAPIClient", config =>
                        {
                            config.BaseAddress = new Uri(apiAddress);
                            config.Timeout = new TimeSpan(0, 0, 30);
                            config.DefaultRequestHeaders.Clear();
                        });
                    }
                );
        }
    }
}