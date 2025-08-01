namespace Pronia.Utilities.Interfaces;

public interface IEmailService
{
   Task SendEmailAsync(string email,string subject,string body,bool isBodyHtml = true);
}