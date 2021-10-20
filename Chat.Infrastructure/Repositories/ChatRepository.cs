using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chat.Application.Repositories;
using Chat.Core.Entities;
using Chat.Infrastructure.Data;
using Chat.Infrastructure.IdentityData;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDataContext _dataContext;
        private readonly ApplicationDataContext dataContext;

        public ChatRepository(ApplicationDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IList<ChatMessage>> GetMessagesAsync(string userName, int numMessages = 50)
        {
            var result = await _dataContext.ChatMessages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Where(x => x.ToUser.UserName == userName)
                .OrderBy(x => x.CreatedDate)
                .Take(numMessages)
                .ToListAsync();

            return result;
        }

        public async Task AddMessage(ChatMessage message)
        {
            await _dataContext.ChatMessages.AddAsync(message);
            await _dataContext.SaveChangesAsync();
        }
    }
}