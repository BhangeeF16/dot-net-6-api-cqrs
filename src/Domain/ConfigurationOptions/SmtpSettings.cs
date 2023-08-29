namespace Domain.ConfigurationOptions;
public class SmtpSettings
{
    public string SenderAddress { get; set; }
    public string DisplayName { get; set; }

    public string UserName { get; set; }
    public string Password { get; set; }

    public string Host { get; set; }
    public int Port { get; set; }

    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public bool IsBodyHtml { get; set; }
}
