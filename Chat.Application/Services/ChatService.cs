using System;
using System.Threading.Tasks;
using BlazorChat.Shared;
using Chat.Application.DTOs;
using Chat.Application.Repositories;
using Microsoft.AspNetCore.Http;

namespace Chat.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IIdentityRepository _identityRepository;

        public ChatService(IChatRepository chatRepository, IIdentityRepository identityRepository)
        {
            _chatRepository = chatRepository;
            _identityRepository = identityRepository;
        }

        public async Task SendMessageAsync(SendMessageDto sendMessageDto)
        {
            if (string.IsNullOrEmpty(sendMessageDto.SenderEmail))
                throw new BadHttpRequestException("sender email address can't be null.");

            if (string.IsNullOrEmpty(sendMessageDto.TargetEmail))
                throw new BadHttpRequestException("target email address can't be null.");

            var sender = await _identityRepository.GetUserByEmailAsync(sendMessageDto.SenderEmail);
            var target = await _identityRepository.GetUserByEmailAsync(sendMessageDto.TargetEmail);

            if (sender is null)
                throw new Exception($"sender user with email address '{sendMessageDto.SenderEmail}' not found.");

            if (target is null)
                throw new Exception($"target user with email address '{sendMessageDto.TargetEmail}' not found.");

            // save history in database
            await _chatRepository.AddMessage(new ChatMessage()
            {
                Message = sendMessageDto.Message,
                CreatedDate = DateTime.Now,
                FromUserId = sender.Id,
                ToUserId = target.Id,
            });

            // Send to Message broker
        }
    }
}