// Services/EmailService.cs
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Agreement.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string ccEmail = null)
        {
            using (var client = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"])))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(
                    _config["Smtp:Username"],
                    _config["Smtp:Password"]);

                var mail = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    mail.CC.Add(ccEmail);
                }

                await client.SendMailAsync(mail);
            }
        }
    }
}