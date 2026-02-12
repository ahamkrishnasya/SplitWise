using SplitWise.Application.DTOs.Users;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<(RegisterUserResponseDto? Data, List<(string Type, string Message)>? Errors)> RegisterAsync(RegisterUserRequestDto request);
    }
}
