using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Infrastructure.Settings;

namespace SplitWise.Infrastructure.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public GmailEmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendEmailVerificationAsync(string toEmail, string verificationUrl)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MoneySplit", _emailSettings.Username));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Verify your email address";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <p>Thank you for registering with Moneysplit.</p>
                    <p>Click the link below to verify your email address:</p>
                    <p><a href=""{verificationUrl}"">{verificationUrl}</a></p>
                    <p>This link expires in 24 hours.</p>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetUrl)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MoneySplit", _emailSettings.Username));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Reset your password";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <p>You requested a password reset for your Moneysplit account.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href=""{resetUrl}"">{resetUrl}</a></p>
                    <p>This link expires in 15 minutes. If you did not request this, you can safely ignore this email.</p>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
