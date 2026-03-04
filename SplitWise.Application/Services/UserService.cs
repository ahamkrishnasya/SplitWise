using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(RegisterUserResponseDto? Data, List<(string Type, string Message)>? Errors)> RegisterAsync(RegisterUserRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();


            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add(("VALIDATION_ERROR", "Email is required"));

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add(("VALIDATION_ERROR", "First name is required"));

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add(("VALIDATION_ERROR", "Last name is required"));

            if (string.IsNullOrWhiteSpace(request.Credentials))
                errors.Add(("VALIDATION_ERROR", "Credentials are required"));

            if (errors.Count > 0)
                return (null, errors);


            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                errors.Add(("EMAIL_ALREADY_EXISTS", "An account with this email already exists"));
                return (null, errors);
            }


            string hashedPassword;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Credentials));
                if (!decoded.TrimStart().StartsWith("{"))
                    decoded = "{" + decoded + "}";
                var json = JsonDocument.Parse(decoded);
                hashedPassword = json.RootElement.GetProperty("hashedPassword").GetString()!;
            }
            catch
            {
                errors.Add(("VALIDATION_ERROR", "Credentials format is invalid"));
                return (null, errors);
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                EmailConfirmed = false
            };

            await _userRepository.AddAsync(user);

            // Generate cryptographically secure, URL-safe verification token
            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var rawToken = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            var verificationToken = new EmailVerificationToken
            {
                UserId = user.Id,
                Token = rawToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false,
                CreatedBy = user.Id
            };

            await _emailVerificationRepository.AddAsync(verificationToken);

            // Send verification email — failure does NOT rollback user or token creation
            try
            {
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
                var verificationUrl = $"{frontendBaseUrl}/verify-email?token={rawToken}";
                await _emailService.SendEmailVerificationAsync(user.Email!, verificationUrl);
            }
            catch
            {
                // Email send failure is non-fatal
            }

            var response = new RegisterUserResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailVerified = user.EmailConfirmed,
                CreatedAt = user.CreatedAt
            };

            return (response, null);
        }
    }
}
