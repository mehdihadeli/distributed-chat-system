using System.Threading.Tasks;
using Chat.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Web.Hubs
{
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext
    //https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/groups
    //https://stackoverflow.com/questions/19522103/signalr-sending-a-message-to-a-specific-user-using-iuseridprovider-new-2-0
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        //Client side apps can call methods that are defined as public on the hub.
        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(SendMessageDto message)
        {
            await Clients.All.SendForReceiveMessage(message);
        }

        // public override Task OnConnectedAsync()
        // {
        //     string name = Context.User.Identity.Name;
        //     Groups.AddToGroupAsync(Context.ConnectionId, name).GetAwaiter().GetResult();
        //     return base.OnConnectedAsync();
        // }
    }

    public interface IChatClient
    {
        Task SendForReceiveMessage(SendMessageDto message);
    }
}