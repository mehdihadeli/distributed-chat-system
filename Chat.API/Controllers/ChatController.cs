using System.Threading.Tasks;
using BlazorChat.Shared;
using Chat.API.Controllers.ViewModels;
using Chat.Application.DTOs;
using Chat.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        HubConnection connection;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatSignalr")
                .Build();
        }

        [HttpPost]
        [Route("send-message")]
        public async Task SendMessageAsync(SendMessageRequest request)
        {
            var dto = new SendMessageDto()
            {
                Message = request.Message,
                SenderEmail = request.SenderEmail,
                TargetEmail = request.TargetEmail
            };
            await _chatService.SendMessageAsync(dto);

            await connection.StartAsync();
            await connection.InvokeAsync("SendMessage",
                new ChatMessage() { Id = 10 }, "mehdi@yahoo.com");
        }
    }
}