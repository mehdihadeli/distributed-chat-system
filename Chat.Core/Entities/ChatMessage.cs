using System;

namespace Chat.Core.Entities
{
    public class ChatMessage
    {
        public long Id { get; init; }
        public string FromUserId { get; init; }
        public string ToUserId { get; init; }
        public string Message { get; init; }
        public DateTime CreatedDate { get; init; }
        public virtual ApplicationUser FromUser { get; init; }
        public virtual ApplicationUser ToUser { get; init; }
    }
}