using Utilities.Models;

namespace Utilities.Abstractions;
public interface IEmailService
{
    Task SendEmailAsync(EmailOptions email);
}
