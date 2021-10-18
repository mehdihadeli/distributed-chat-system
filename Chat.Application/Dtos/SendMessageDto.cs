namespace Chat.Application.DTOs
{
    public class SendMessageDto
    {
        public string TargetEmail { get; init; }
        public string SenderEmail { get; init; }
        public string Message { get; init; }
    }
}