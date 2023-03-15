namespace Application.Modules.PostalCodes.Models
{
    public class SmtpCredentialModel
    {
        public string? FromMail { get; set; }
        public string? SmtpClient { get; set; }
        public string? SmtpUser { get; set; }
        public string? SmtpPort { get; set; }
        public string? SmtpPassword { get; set; }
    }
}
