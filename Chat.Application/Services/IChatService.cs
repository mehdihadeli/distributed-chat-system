using System.Threading.Tasks;
using Chat.Application.DTOs;

namespace Chat.Application.Services
{
    public interface IChatService
    {
        Task SendMessageAsync(SendMessageDto sendMessageDto);
    }
}