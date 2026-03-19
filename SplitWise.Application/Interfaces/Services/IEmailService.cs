using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string verificationUrl);
        Task SendPasswordResetEmailAsync(string toEmail, string resetUrl);
        Task SendEmailInvitationAsync(string toEmail, string invitationUrl,User user);
    }
}
