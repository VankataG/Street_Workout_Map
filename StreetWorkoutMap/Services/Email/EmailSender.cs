using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace StreetWorkoutMap.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var appPassword =
                configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException(
                    "Email App Password is not configured.");
            }

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    "sw.map.eu@gmail.com",
                    appPassword)
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    "sw.map.eu@gmail.com",
                    "SW-MAP"),

                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
