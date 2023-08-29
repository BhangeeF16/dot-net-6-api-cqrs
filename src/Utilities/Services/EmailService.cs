using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using Utilities.Abstractions;
using Utilities.Models;

namespace Utilities.Services;
public class EmailService : IEmailService
{
    private readonly SmtpClient _client;
    private readonly SmtpSettings _options;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _options = options.Value;
        _client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)
        };
    }

    public async Task SendEmailAsync(EmailOptions email)
    {
        var mail = new MailMessage()
        {
            Body = email.Body,
            Subject = email.Subject,
            IsBodyHtml = email.IsBodyHtml,
            BodyEncoding = email.BodyEncoding,
            From = new MailAddress(_options.SenderAddress, _options.DisplayName, Encoding.Default),
        };

        if (email.PlaceHolders != null && email.PlaceHolders.Any())
        {
            mail.Body = mail.Body.ReplacePlaceHolders(email.PlaceHolders);
        }

        foreach (var to in email.ToEmails)
        {
            mail.To.Add(to);
        }

        if (email.CCEmails != null && email.CCEmails.Any())
        {
            foreach (var cc in email.CCEmails)
            {
                mail.CC.Add(cc);
            }
        }

        if (email.BCCEmails != null && email.BCCEmails.Any())
        {
            foreach (var bcc in email.BCCEmails)
            {
                mail.Bcc.Add(bcc);
            }
        }

        try
        {
            await _client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
