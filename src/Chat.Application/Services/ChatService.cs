using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Application.DTOs;
using Chat.Application.Repositories;
using Chat.Core.Entities;
using Humanizer;

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

        public async Task<long> SendMessageAsync(SendMessageDto sendMessageDto)
        {
            if (string.IsNullOrEmpty(sendMessageDto.SenderUserName))
                throw new BadRequestException("sender userName can't be null.");

            if (string.IsNullOrEmpty(sendMessageDto.TargetUserName))
                throw new BadRequestException("target userName can't be null.");

            var sender = await _identityRepository.GetUserByNameAsync(sendMessageDto.SenderUserName);
            var target = await _identityRepository.GetUserByNameAsync(sendMessageDto.TargetUserName);

            if (sender is null)
                throw new AppException($"sender user with userName '{sendMessageDto.SenderUserName}' not found.");

            if (target is null)
                throw new AppException($"target user with userName '{sendMessageDto.TargetUserName}' not found.");

            var messageHistory = new ChatMessage
            {
                Message = sendMessageDto.Message,
                CreatedDate = DateTime.Now,
                FromUserId = sender.Id,
                ToUserId = target.Id,
                FromUser = sender,
                ToUser = target
            };

            // Save message history (in-memory repository or efcore-repository)
            await _chatRepository.AddMessage(messageHistory);


            // Send to Message broker
            _natsBus.Publish(new ChatMessageDto
            {
                Message = sendMessageDto.Message,
                MessageDate = messageHistory.CreatedDate,
                SenderUserName = sendMessageDto.SenderUserName,
                TargetUserName = target.UserName
            }, nameof(ChatMessage).Underscore());

            return messageHistory.Id;
        }

        public async Task<IEnumerable<ChatMessageDto>> LoadMessagesByCount(string userName, int? numMessages)
        {
            var result = (await _chatRepository.GetMessagesAsync(userName, numMessages)).Select(x => new ChatMessageDto
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
            var result = (await _chatRepository.GetMessagesFromDateAsync(userName, dateTime)).Select(x => new ChatMessageDto
            {
                Message = x.Message,
                MessageDate = x.CreatedDate,
                SenderUserName = x.FromUser.UserName,
                TargetUserName = x.ToUser.UserName
            });

            return result;
        }

        public async Task<ChatMessageDto> GetChatById(long id)
        {
           var message = await _chatRepository.GetById(id);
           return new ChatMessageDto
           {
               Message = message.Message,
               MessageDate = message.CreatedDate,
               SenderUserName = message.FromUser.UserName,
               TargetUserName = message.ToUser.UserName
           };
        }
    }
}