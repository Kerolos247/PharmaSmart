using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApplication4.Application.Common.Interfaces;

namespace WebApplication4.Infrastructure.Common
{
    public class BrevoService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public BrevoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
           
            var smtpLogin = _configuration["SmtpSettings:Login"];
            var smtpPassword = _configuration["SmtpSettings:Password"];
            var smtpServer = _configuration["SmtpSettings:Server"] ?? "smtp-relay.brevo.com";
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");

           
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? "kerolos.adel754@gmail.com";

            using var message = new MailMessage();

           
            message.From = new MailAddress(senderEmail, "Smart Pharmacy System");
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

           
            var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpLogin, smtpPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}