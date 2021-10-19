using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Core.Entities;

namespace Chat.Application.Repositories
{
    public interface IChatRepository
    {
        Task<IList<ChatMessage>> GetMessagesAsync(int? numberOfMessages = 0);
        Task AddMessage(ChatMessage message);
    }
}