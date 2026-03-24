using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplitWise.Application.DTOs.EmailChange;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class EmailChangeService : IEmailChangeService
    {
        private readonly IEmailChangeRepository _emailChangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EmailChangeService(
            IEmailChangeRepository emailChangeRepository,
            IUserRepository userRepository,
            ILoginRepository loginRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _emailChangeRepository = emailChangeRepository;
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> RequestEmailChangeAsync(RequestEmailChangeDto request)
        {
            if (string.IsNullOrWhiteSpace(request.NewEmail))
                return (false, new List<(string, string)> { ("ValidationError", "New email is required.") });

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return (false, new List<(string, string)> { ("ValidationError", "Current password is required.") });

            var userId = _loginRepository.GetUserId();
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                return (false, new List<(string, string)> { ("NotFound", "User not found.") });

            string submittedHash;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.CurrentPassword));
                if (!decoded.TrimStart().StartsWith("{"))
                    decoded = "{" + decoded + "}";
                var json = JsonDocument.Parse(decoded);
                submittedHash = json.RootElement.GetProperty("hashedPassword").GetString()!;
            }
            catch
            {
                return (false, new List<(string, string)> { ("ValidationError", "Current password format is invalid.") });
            }

            if (submittedHash != user.PasswordHash)
                return (false, new List<(string, string)> { ("InvalidCredentials", "Current password is incorrect.") });

            var newEmail = request.NewEmail.Trim().ToLower();

            if (newEmail == user.Email!.ToLower())
                return (false, new List<(string, string)> { ("ValidationError", "New email must be different from your current email.") });

            var existing = await _userRepository.GetByEmailAsync(newEmail);
            if (existing != null)
                return (false, new List<(string, string)> { ("EmailAlreadyExists", "An account with this email already exists.") });

            await _emailChangeRepository.DeleteByUserIdAsync(userId);

            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var rawToken = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            var changeToken = new EmailChangeToken
            {
                UserId = userId,
                Token = rawToken,
                NewEmail = newEmail,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                CreatedBy = userId
            };

            await _emailChangeRepository.AddAsync(changeToken);
            await _emailChangeRepository.SaveChangesAsync();

            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
            var confirmationUrl = $"{frontendBaseUrl}/confirm-email-change?token={rawToken}";

            try { await _emailService.SendEmailChangeVerificationAsync(newEmail, confirmationUrl); }
            catch { }

            try { await _emailService.SendEmailChangeNotificationAsync(user.Email!); }
            catch { }

            return (true, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> ConfirmEmailChangeAsync(ConfirmEmailChangeDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return (false, new List<(string, string)> { ("ValidationError", "Token is required.") });

            var changeToken = await _emailChangeRepository.GetByTokenAsync(request.Token);

            if (changeToken == null)
                return (false, new List<(string, string)> { ("InvalidToken", "Email change token is invalid or does not exist.") });

            if (changeToken.ExpiresAt < DateTime.UtcNow)
            {
                _emailChangeRepository.Delete(changeToken);
                await _emailChangeRepository.SaveChangesAsync();
                return (false, new List<(string, string)> { ("TokenExpired", "Email change token has expired.") });
            }

            var existing = await _userRepository.GetByEmailAsync(changeToken.NewEmail);
            if (existing != null)
                return (false, new List<(string, string)> { ("EmailAlreadyExists", "This email address is no longer available.") });

            changeToken.User.Email = changeToken.NewEmail;
            changeToken.User.LastModifiedAt = DateTime.UtcNow;
            changeToken.User.LastModifiedBy = changeToken.UserId;

            _emailChangeRepository.Delete(changeToken);
            await _emailChangeRepository.SaveChangesAsync();

            return (true, null);
        }
    }
}
