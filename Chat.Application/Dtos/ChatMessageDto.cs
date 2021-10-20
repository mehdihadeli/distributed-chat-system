using System;

namespace Chat.Application.DTOs
{
    public class ChatMessageDto
    {
        public string TargetUserName { get; init; }
        public string SenderUserName { get; init; }
        public string Message { get; init; }
        public DateTime MessageDate { get; init; }
    }
}