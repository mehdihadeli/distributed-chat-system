using System.Threading.Tasks;
using BlazorChat.Shared;
using Chat.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Web.Controllers
{
    [Route("[controller]")]
    public class SignalRController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("test")]
        public async Task Test()
        {
            await _hubContext.Clients.All
                .SendAsync("SendForReceiveMessage", new ChatMessage() { Id = 22 }, "mehdi@yahoo.com");
        }
    }
}