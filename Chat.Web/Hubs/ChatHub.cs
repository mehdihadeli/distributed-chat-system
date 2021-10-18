using System.Threading.Tasks;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Web.Hubs
{
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext
    //https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client
    //https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client
    public class ChatHub : Hub<IChatClient>
    {
        //Client side apps can call methods that are defined as public on the hub.
        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(ChatMessage message, string targetUserEmail)
        {
            await Clients.All.SendForReceiveMessage(message, targetUserEmail);
        }

        [HubMethodName("ChatNotification")]
        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.All.ChatNotificationAsync(message, receiverUserId, senderUserId);
        }
    }
    public interface IChatClient
    {
        Task SendForReceiveMessage(ChatMessage message, string userName);
        Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId);
    }
}