using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PasswordResetService(
            IPasswordResetRepository passwordResetRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _passwordResetRepository = passwordResetRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> RequestPasswordResetAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, new List<(string, string)>
                {
                    ("VALIDATION_ERROR", "Email is required.")
                });
            }

            var user = await _userRepository.GetByEmailAsync(email);

            // Do not reveal whether the email exists or is confirmed
            if (user == null || !user.EmailConfirmed)
                return (true, null);

            // Delete any existing reset tokens for this user
            await _passwordResetRepository.DeleteTokensByUserIdAsync(user.Id);

            // Generate a cryptographically secure, URL-safe token
            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var rawToken = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = rawToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                CreatedBy = user.Id
            };

            await _passwordResetRepository.AddAsync(resetToken);
            await _passwordResetRepository.SaveChangesAsync();

            // Send the reset email — failure is non-fatal
            try
            {
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
                var resetUrl = $"{frontendBaseUrl}/reset-password?token={rawToken}";
                await _emailService.SendPasswordResetEmailAsync(user.Email!, resetUrl);
            }
            catch
            {
                // Email send failure does not affect the response
            }

            return (true, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> ResetPasswordAsync(string token, string newPasswordBase64)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return (false, new List<(string, string)>
                {
                    ("VALIDATION_ERROR", "Token is required.")
                });
            }

            if (string.IsNullOrWhiteSpace(newPasswordBase64))
            {
                return (false, new List<(string, string)>
                {
                    ("VALIDATION_ERROR", "New password is required.")
                });
            }

            var resetToken = await _passwordResetRepository.GetByTokenAsync(token);

            if (resetToken == null)
            {
                return (false, new List<(string, string)>
                {
                    ("INVALID_TOKEN", "Password reset token is invalid or does not exist.")
                });
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                _passwordResetRepository.Delete(resetToken);
                await _passwordResetRepository.SaveChangesAsync();

                return (false, new List<(string, string)>
                {
                    ("TOKEN_EXPIRED", "Password reset token has expired.")
                });
            }

            string newPasswordHash;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(newPasswordBase64));
                if (!decoded.TrimStart().StartsWith("{"))
                    decoded = "{" + decoded + "}";
                var json = JsonDocument.Parse(decoded);
                newPasswordHash = json.RootElement.GetProperty("hashedPassword").GetString()!;
            }
            catch
            {
                return (false, new List<(string, string)>
                {
                    ("VALIDATION_ERROR", "New password format is invalid.")
                });
            }

            resetToken.User.PasswordHash = newPasswordHash;
            _passwordResetRepository.Delete(resetToken);
            await _passwordResetRepository.SaveChangesAsync();

            return (true, null);
        }
    }
}
