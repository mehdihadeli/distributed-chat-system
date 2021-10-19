using System.Threading.Tasks;
using Chat.Application.DTOs;
using Chat.Core.Entities;
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
    public class ChatHub : Hub<IChatClient>
    {
        //Client side apps can call methods that are defined as public on the hub.
        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(SendMessageDto message)
        {
            await Clients.All.SendForReceiveMessage(message);
        }

        [HubMethodName("ChatNotification")]
        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.All.ChatNotificationAsync(message, receiverUserId, senderUserId);
        }
    }
    public interface IChatClient
    {
        Task SendForReceiveMessage(SendMessageDto message);
        Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId);
    }
}