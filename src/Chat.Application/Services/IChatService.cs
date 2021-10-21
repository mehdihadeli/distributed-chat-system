using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Application.DTOs;

namespace Chat.Application.Services
{
    public interface IChatService
    {
        Task<long> SendMessageAsync(SendMessageDto sendMessageDto);
        Task<IEnumerable<ChatMessageDto>> LoadMessagesByCount(string userName, int numMessages = 50);
        Task<IEnumerable<ChatMessageDto>> LoadMessagesByTime(string userName, DateTime dateTime);
        Task<ChatMessageDto> GetChatById(long id);
    }
}