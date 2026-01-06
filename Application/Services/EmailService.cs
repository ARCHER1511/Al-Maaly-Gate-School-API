using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
        {
            var smtp = new SmtpClient
            {
                Host = _config["Email:Smtp:Host"]!,
                Port = int.Parse(_config["Email:Smtp:Port"]!),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config["Email:Smtp:Username"],
                    _config["Email:Smtp:Password"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:From"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            await smtp.SendMailAsync(message);
        }
    }
}
