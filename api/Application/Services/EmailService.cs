
using System.Net;
using System.Net.Mail;
using ecommapi.Application.Configurations;
using ecommapi.Application.Interfaces;
using ecommapi.Domain.Models;
using MimeKit;


namespace ecommapi.Application.Services
{
    public class EmailService : IEmailService
    {
        //private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _mailSettings;
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _mailSettings = config.GetSection("MailSettings").Get<MailSettings>() ?? throw new ArgumentNullException(nameof(config), "MailSettings section is missing or invalid.");
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                // MimeMessage email = new MimeMessage();
                // email.From.Add(MailboxAddress.Parse(emailDto.From));
                // email.To.Add(MailboxAddress.Parse(emailDto.To));
                // email.Subject = emailDto.Subject;
                // email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDto.Body };

                using var smtpClient = new SmtpClient(_mailSettings.Server, _mailSettings.Port)
                {
                    Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password),
                    EnableSsl = _mailSettings.EnableSSL
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSettings.FromMail!, _mailSettings.FromName),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(emailDto.ToEmail);
                await smtpClient.SendMailAsync(mailMessage);
                //await smtpClient.DisconnectAsync(true);
                _logger.LogInformation("Email sent successfully to {ToEmail}", emailDto.ToEmail);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending email to {ToEmail}", emailDto.ToEmail);
                throw;
            }
        }
    }
}