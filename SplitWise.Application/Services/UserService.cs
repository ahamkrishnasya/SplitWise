using System.Text;
using System.Text.Json;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
