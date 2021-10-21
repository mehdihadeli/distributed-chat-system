using System;

namespace Chat.Application.DTOs
{
    public class SendMessageDto
    {
        public string TargetUserName { get; init; }
        public string SenderUserName { get; init; }
        public string Message { get; init; }
    }
}