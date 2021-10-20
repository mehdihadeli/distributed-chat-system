using Microsoft.AspNetCore.SignalR;

namespace Chat.Web
{
    //https://stackoverflow.com/questions/19522103/signalr-sending-a-message-to-a-specific-user-using-iuseridprovider-new-2-0
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            //UserName
           return connection.User?.Identity?.Name;
        }
    }
}