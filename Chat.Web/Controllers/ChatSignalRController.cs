using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Chat.Application.DTOs;
using Chat.Web.Hubs;
using Chat.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Web.Controllers
{
    [Route("[controller]")]
    public class ChatSignalRController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        HubConnection connection;

        public ChatSignalRController(IHubContext<ChatHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatSignalr")
                .Build();
        }

        [HttpPost("send-message")]
        public async Task<ActionResult> SendMessageAsync(SendMessageRequest request)
        {
            var sendMessageDto = new SendMessageDto
            {
                Message = request.Message,
                SenderUserName = request.SenderUserName,
                TargetUserName = request.TargetUserName
            };

            var client = _httpClientFactory.CreateClient("WebAPIClient");
            var postResponse = (await client.PostAsJsonAsync("api/chat/send-message", sendMessageDto));
            postResponse.EnsureSuccessStatusCode();
            var chatId = await postResponse.Content.ReadFromJsonAsync<int>();

            // directly send to signalr hub instead of broker
            var chatMessageDto = await client.GetFromJsonAsync<ChatMessageDto>($"api/chat/messages/{chatId}");
            await connection.StartAsync();
            await connection.InvokeAsync("SendMessage", chatMessageDto);

            // // or
            // await _hubContext.Clients.All
            //     .SendAsync("SendForReceiveMessage", chatMessageDto);

            return NoContent();
        }
    }
}