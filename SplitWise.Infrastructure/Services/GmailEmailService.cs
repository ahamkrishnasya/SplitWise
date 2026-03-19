using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
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

        public async Task SendEmailInvitationAsync(string toEmail, string verificationUrl, User user)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MoneySplit", _emailSettings.Username));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = $"{user.FirstName} {user.LastName} invited you to MoneySplit";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; line-height:1.6'>
                    <h2>You're invited to join MoneySplit!</h2>

                    <p><strong>{user.FirstName} {user.LastName}</strong> invited you to join MoneySplit.</p>

                    <p>MoneySplit helps you easily split expenses with friends and family.</p>

                    <p>
                        Click the button below to accept the invitation and verify your email:
                    </p>

                    <p>
                        <a href='{verificationUrl}' 
                           style='background-color:#9147ff;
                                  color:white;
                                  padding:12px 20px;
                                  text-decoration:none;
                                  border-radius:6px;
                                  display:inline-block;'>
                           Accept Invitation
                        </a>
                    </p>

                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p>{verificationUrl}</p>

                    <p>Thanks,<br/>MoneySplit Team</p>
                </div>"
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
