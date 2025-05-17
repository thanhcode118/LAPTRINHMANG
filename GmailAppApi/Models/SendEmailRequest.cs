namespace GmailAppApi.Models
{
    public class SendEmailRequest
    {
        public string? FromEmail { get; set; }
        public string? AppPassword { get; set; }
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
