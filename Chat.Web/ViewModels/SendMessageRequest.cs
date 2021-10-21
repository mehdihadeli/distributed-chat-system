namespace Chat.Web.ViewModels
{
    public class SendMessageRequest
    {
        public string TargetUserName { get; set; }
        public string SenderUserName { get; set; }
        public string Message { get; set; }
    }
}