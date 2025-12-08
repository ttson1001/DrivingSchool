
using System.Threading.Tasks;

namespace TutorDrive.Services.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendOtpEmailAsync(string email, string otp);
    }
}

