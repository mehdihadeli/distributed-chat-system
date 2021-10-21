using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chat.Application.Repositories;
using Chat.Core.Entities;

namespace Chat.Infrastructure.Repositories
{
    public class InMemoryChatRepository : IChatRepository
    {
        private static readonly List<ChatMessage> InMemoryMessageHistory = new();

        public Task<IList<ChatMessage>> GetMessagesAsync(string userName, int numMessages = 50)
        {
            var result = InMemoryMessageHistory
                .Where(x => x.ToUser.UserName == userName || x.FromUser.UserName == userName)
                .OrderBy(x => x.CreatedDate)
                .Take(numMessages).ToList() as IList<ChatMessage>;

            return Task.FromResult(result);
        }

        public Task<IList<ChatMessage>> GetMessagesFromDateAsync(string userName, DateTime fromDate)
        {
            var result = InMemoryMessageHistory
                .Where(x => x.ToUser.UserName == userName || x.FromUser.UserName == userName)
                .Where(x => x.CreatedDate >= fromDate)
                .OrderBy(x => x.CreatedDate)
                .ToList() as IList<ChatMessage>;

            return Task.FromResult(result);
        }

        public Task AddMessage(ChatMessage message)
        {
            var newId = InMemoryMessageHistory.LastOrDefault()?.Id ?? 0 + 1;
            typeof(ChatMessage).GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(message, newId, null);
            InMemoryMessageHistory.Add(message);

            return Task.CompletedTask;
        }

        public Task<ChatMessage> GetById(long id)
        {
            return Task.FromResult(InMemoryMessageHistory.SingleOrDefault(x => x.Id == id));
        }
    }
}