using Microsoft.Extensions.Configuration;
using SplitWise.Application.DTOs.EmailVerification;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SplitWise.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILoginRepository _loginRepository;

        public UserService(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILoginRepository loginRepository)
        {
            _userRepository = userRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _emailService = emailService;
            _configuration = configuration;
            _loginRepository = loginRepository;
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

            try
            {
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
                var verificationUrl = $"{frontendBaseUrl}/verify-email?token={rawToken}";
                await _emailService.SendEmailVerificationAsync(user.Email!, verificationUrl);
            }
            catch
            {

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

        public async Task<(UserProfileResponseDto? Data, List<(string Type, string Message)>? Errors)> GetProfileAsync()
        {
            var errors = new List<(string Type, string Message)>();

            var userId = _loginRepository.GetUserId();
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                errors.Add(("NotFound", "User not found."));
                return (null, errors);
            }

            return (MapToProfileDto(user), null);
        }

        public async Task<(UserProfileResponseDto? Data, List<(string Type, string Message)>? Errors)> UpdateProfileAsync(PatchUserRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();

            var firstName = request.FirstName?.Trim();
            var lastName = request.LastName?.Trim();

            var hasFirst = !string.IsNullOrWhiteSpace(firstName);
            var hasLast = !string.IsNullOrWhiteSpace(lastName);

            if (!hasFirst && !hasLast)
            {
                errors.Add(("ValidationError", "At least one of FirstName or LastName must be provided."));
                return (null, errors);
            }

            if (request.FirstName is not null && !hasFirst)
            {
                errors.Add(("ValidationError", "FirstName cannot be empty or whitespace."));
                return (null, errors);
            }

            if (request.LastName is not null && !hasLast)
            {
                errors.Add(("ValidationError", "LastName cannot be empty or whitespace."));
                return (null, errors);
            }

            var userId = _loginRepository.GetUserId();

            var (user, repoErrors) = await _userRepository.UpdateNameAsync(userId, firstName, lastName);

            if (repoErrors != null && repoErrors.Count > 0)
                return (null, repoErrors);

            return (MapToProfileDto(user!), null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return (false, new List<(string, string)> { ("ValidationError", "Current password is required.") });

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return (false, new List<(string, string)> { ("ValidationError", "New password is required.") });

            string currentHash;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.CurrentPassword));
                if (!decoded.TrimStart().StartsWith('{'))
                    decoded = "{" + decoded + "}";
                var json = JsonDocument.Parse(decoded);
                currentHash = json.RootElement.GetProperty("hashedPassword").GetString()!;
            }
            catch
            {
                return (false, new List<(string, string)> { ("ValidationError", "Current password format is invalid.") });
            }

            string newHash;
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.NewPassword));
                if (!decoded.TrimStart().StartsWith('{'))
                    decoded = "{" + decoded + "}";
                var json = JsonDocument.Parse(decoded);
                newHash = json.RootElement.GetProperty("hashedPassword").GetString()!;
            }
            catch
            {
                return (false, new List<(string, string)> { ("ValidationError", "New password format is invalid.") });
            }

            if (currentHash == newHash)
                return (false, new List<(string, string)> { ("ValidationError", "New password must be different from your current password.") });

            var userId = _loginRepository.GetUserId();
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                return (false, new List<(string, string)> { ("NotFound", "User not found.") });

            if (currentHash != user.PasswordHash)
                return (false, new List<(string, string)> { ("InvalidCredentials", "Current password is incorrect.") });

            return await _userRepository.UpdatePasswordAsync(userId, newHash);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> DeleteAccountAsync(DeleteAccountRequestDto request)
        {
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
                if (!decoded.TrimStart().StartsWith('{'))
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

            var originalEmail = user.Email!;

            var (success, errors) = await _userRepository.DeleteAccountAsync(userId);
            if (!success)
                return (false, errors);

            try { await _emailService.SendAccountDeletionAsync(originalEmail); }
            catch { }

            return (true, null);
        }
        private static UserProfileResponseDto MapToProfileDto(Domain.Entities.User user) =>
            new UserProfileResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt
            };
    }
}
