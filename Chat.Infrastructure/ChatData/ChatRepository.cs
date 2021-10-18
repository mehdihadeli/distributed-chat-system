using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorChat.Shared;
using Chat.Application.Repositories;
using Chat.Infrastructure.IdentityData;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.ChatData
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDataContext _chatDataContext;
        private readonly IdentityDataContext _identityDataContext;

        public ChatRepository(ChatDataContext chatDataContext, IdentityDataContext identityDataContext)
        {
            _chatDataContext = chatDataContext;
            _identityDataContext = identityDataContext;
        }

        public async Task<IList<ChatMessage>> GetMessagesAsync(int? numberOfMessages = null)
        {
            var result = await _chatDataContext.ChatMessages.OrderBy(x => x.CreatedDate)
                .Take(numberOfMessages ?? 50)
                .ToListAsync();

            return result;
        }

        public async Task AddMessage(ChatMessage message)
        {
            await _chatDataContext.ChatMessages.AddAsync(message);
            await _chatDataContext.SaveChangesAsync();
        }
    }
}