using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CustomIdentityServer.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class MockEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n📧 MOCK EMAIL SENT:\nTo: {to}\nSubject: {subject}\nBody: {body}\n");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];
            var smtpEnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "true");
            var from = _configuration["Smtp:From"] ?? smtpUser ?? "no-reply@example.com";

            using var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass),
                EnableSsl = smtpEnableSsl
            };
            var mail = new System.Net.Mail.MailMessage(from, to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}
