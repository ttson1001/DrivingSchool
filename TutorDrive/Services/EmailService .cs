using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TutorDrive.Services.IService.TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            string subject = "TutorDrive - OTP đặt lại mật khẩu";

            string body = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>OTP Đặt Lại Mật Khẩu</h2>
            <p>Mã OTP của bạn là:</p>
            <h1 style='color:#2d89ef; letter-spacing: 4px;'>{otp}</h1>
            <p>OTP có hiệu lực trong 5 phút.</p>
            <hr />
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        </div>
    ";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
