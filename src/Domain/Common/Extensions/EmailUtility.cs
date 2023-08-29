using Domain.Entities.GeneralModule;
using System.Net;
using System.Net.Mail;

namespace Domain.Common.Extensions;

public class EmailUtility
{
    public static void SendMail(string toEmail, string body, string subject, string messsage, List<AppSetting> appSettings)
    {
        try
        {

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(appSettings.FirstOrDefault(c => c.Name == "FromMail")?.Value, "Gig Panel");
                mail.To.Add(new MailAddress(toEmail, body));
                mail.Subject = subject;
                mail.Body = messsage;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new(appSettings.FirstOrDefault(c => c.Name == "SmtpClient")?.Value, Convert.ToInt32(appSettings.FirstOrDefault(c => c.Name == "SmtpPort")?.Value)))
                {
                    smtp.Credentials = new NetworkCredential(appSettings.FirstOrDefault(c => c.Name == "SmtpUser")?.Value, appSettings.FirstOrDefault(c => c.Name == "SmtpPassword")?.Value);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static string GetEmailTemplateFromFile(string EmailTemplateName)
    {
        var htmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"EmailTemplates\{EmailTemplateName}.html");
        return File.ReadAllText(htmlFile);
    }

}