using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Application.DTOs;
using Chat.Application.Repositories;
using Chat.Core.Entities;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace Chat.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly INatsBus _natsBus;

        public ChatService(IChatRepository chatRepository, IIdentityRepository identityRepository, INatsBus natsBus)
        {
            _chatRepository = chatRepository;
            _identityRepository = identityRepository;
            _natsBus = natsBus;
        }

        public async Task SendMessageAsync(SendMessageDto sendMessageDto)
        {
            if (string.IsNullOrEmpty(sendMessageDto.SenderUserName))
                throw new BadHttpRequestException("sender userName can't be null.");

            if (string.IsNullOrEmpty(sendMessageDto.TargetUserName))
                throw new BadHttpRequestException("target userName can't be null.");

            var sender = await _identityRepository.GetUserByNameAsync(sendMessageDto.SenderUserName);
            var target = await _identityRepository.GetUserByNameAsync(sendMessageDto.TargetUserName);

            if (sender is null)
                throw new Exception($"sender user with userName '{sendMessageDto.SenderUserName}' not found.");

            if (target is null)
                throw new Exception($"target user with userName '{sendMessageDto.TargetUserName}' not found.");

            // Save message history in database
            var messageHistory = new ChatMessage
            {
                Message = sendMessageDto.Message,
                CreatedDate = DateTime.Now,
                FromUserId = sender.Id,
                ToUserId = target.Id,
            };
            await _chatRepository.AddMessage(messageHistory);

            // Send to Message broker
            _natsBus.Publish(new ChatMessageDto
            {
                Message = sendMessageDto.Message,
                MessageDate = messageHistory.CreatedDate,
                SenderUserName = sendMessageDto.SenderUserName,
                TargetUserName = target.UserName
            }, nameof(ChatMessage).Underscore());
        }

        public Task<IEnumerable<ChatMessageDto>> LoadMessages(string userId)
        {
            return null;
        }
    }
}