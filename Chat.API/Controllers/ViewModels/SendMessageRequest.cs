namespace Chat.API.Controllers.ViewModels
{
    public class SendMessageRequest
    {
        public string TargetEmail { get; set; }
        public string SenderEmail { get; set; }
        public string Message { get; set; }
    }
}