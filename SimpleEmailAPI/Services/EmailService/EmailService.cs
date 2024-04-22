
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace SimpleEmailAPI.Services.EmailService
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        private readonly IConfiguration _config = config;

        public void SendEmail(EmailDto request)
        {
            var username = _config.GetSection("EmailSettings").GetSection("Username").Value;
            var password = _config.GetSection("EmailSettings").GetSection("Password").Value;
            int port = int.TryParse(_config.GetSection("EmailSettings").GetSection("Port").Value, out int parsedPort) ? parsedPort : 587;
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(username));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _config.GetSection("EmailSettings").GetSection("Host").Value,
                port, 
                SecureSocketOptions.StartTls
               );
            smtp.Authenticate(username, password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
