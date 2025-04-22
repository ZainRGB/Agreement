using System.Threading.Tasks;

namespace Agreement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string ccEmail = null);
    }
}