using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Core.Entities;

namespace Chat.Application.Repositories
{
    public interface IChatRepository
    {
        Task<IList<ChatMessage>> GetMessagesAsync(string userName, int? numMessages);
        Task<IList<ChatMessage>> GetMessagesFromDateAsync(string userName, DateTime fromDate);
        Task AddMessage(ChatMessage message);
        public Task<ChatMessage> GetById(long id);
    }
}