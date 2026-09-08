namespace MaleFashion.Application.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);

}
