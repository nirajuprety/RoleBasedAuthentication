
using System.Net.Mail;
using System.Net;

namespace RolesBaseIdentification.Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true; 

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false, 
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
