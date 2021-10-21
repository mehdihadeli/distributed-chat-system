using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Application.Repositories;
using Chat.Core.Entities;
using Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class EFChatRepository : IChatRepository
    {
        private readonly ApplicationDataContext _dataContext;

        public EFChatRepository(ApplicationDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IList<ChatMessage>> GetMessagesAsync(string userName, int? numMessages)
        {
            var query = _dataContext.ChatMessages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Where(x => x.ToUser.UserName == userName || x.FromUser.UserName == userName)
                .OrderBy(x => x.CreatedDate);

            if (numMessages is not null)
                return await query.Take((int)numMessages).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<IList<ChatMessage>> GetMessagesFromDateAsync(string userName, DateTime fromDate)
        {
            var result = await _dataContext.ChatMessages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Where(x => x.ToUser.UserName == userName || x.FromUser.UserName == userName)
                .Where(x => x.CreatedDate >= fromDate)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

            return result;
        }

        public async Task AddMessage(ChatMessage message)
        {
            await _dataContext.ChatMessages.AddAsync(message);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<ChatMessage> GetById(long id)
        {
            return await _dataContext.ChatMessages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}