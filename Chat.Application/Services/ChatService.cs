using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly List<ChatMessage> InMemoryMessageHistory = new();

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

            var messageHistory = new ChatMessage
            {
                Message = sendMessageDto.Message,
                CreatedDate = DateTime.Now,
                FromUserId = sender.Id,
                ToUserId = target.Id,
                FromUser = sender,
                ToUser = target
            };

            // // Option 1: Save message history in database
            // await _chatRepository.AddMessage(messageHistory);

            // Option 2: Save message history in-memory in a static field
            InMemoryMessageHistory.Add(messageHistory);

            // Send to Message broker
            _natsBus.Publish(new ChatMessageDto
            {
                Message = sendMessageDto.Message,
                MessageDate = messageHistory.CreatedDate,
                SenderUserName = sendMessageDto.SenderUserName,
                TargetUserName = target.UserName
            }, nameof(ChatMessage).Underscore());
        }

        public async Task<IEnumerable<ChatMessageDto>> LoadMessagesByCount(string userName, int numMessages = 50)
        {
            // Option 1 : Reading received messages form message history in database
            // var result = (await _chatRepository.GetMessagesAsync(userName, numMessages)).Select(x => new ChatMessageDto
            // {
            //     Message = x.Message,
            //     MessageDate = x.CreatedDate,
            //     SenderUserName = x.FromUser.UserName,
            //     TargetUserName = x.ToUser.UserName
            // });

            // Option 2: Reading received messages form message history in in-memory static field
            var result = GetReceivedMessagesByCountInMemory(userName, numMessages).Select(x => new ChatMessageDto
            {
                Message = x.Message,
                MessageDate = x.CreatedDate,
                SenderUserName = x.FromUser.UserName,
                TargetUserName = x.ToUser.UserName
            });

            return result;
        }

        public async Task<IEnumerable<ChatMessageDto>> LoadMessagesByTime(string userName, DateTime dateTime)
        {
            // Option 2: Reading received messages form message history in in-memory static field
            var result = GetReceivedMessagesByTimeInMemory(userName, dateTime).Select(x => new ChatMessageDto
            {
                Message = x.Message,
                MessageDate = x.CreatedDate,
                SenderUserName = x.FromUser.UserName,
                TargetUserName = x.ToUser.UserName
            });

            return result;
        }

        private IList<ChatMessage> GetReceivedMessagesByCountInMemory(string userName, int numMessages = 50)
        {
            return InMemoryMessageHistory
                .Where(x => x.ToUser.UserName == userName)
                .OrderBy(x => x.CreatedDate)
                .Take(numMessages).ToList();
        }

        private IList<ChatMessage> GetReceivedMessagesByTimeInMemory(string userName, DateTime dateTime)
        {
            return InMemoryMessageHistory
                .Where(x => x.ToUser.UserName == userName)
                .Where(x => x.CreatedDate >= dateTime)
                .OrderBy(x => x.CreatedDate).ToList();
        }
    }
}