using System.Net;
using System.Net.Mail;
using Pronia.Utilities.Interfaces;

namespace Pronia.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendEmailAsync(string email,string subject,string body,bool isBodyHtml = true)
    {
        SmtpClient smtpClient = new SmtpClient(configuration["ApplicationEmail:Host"],
            Convert.ToInt32(configuration["ApplicationEmail:Port"]));

        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(configuration["ApplicationEmail:Email"],configuration["ApplicationEmail:Password"]);
        
        MailAddress from = new MailAddress(configuration["ApplicationEmail:Email"],"Pronia");
        MailAddress to = new MailAddress(email);
        MailMessage mailMessage = new MailMessage(from, to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        };
        await smtpClient.SendMailAsync(mailMessage);
    }
    
} 