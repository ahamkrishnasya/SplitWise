using SplitWise.Application.DTOs.Users;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<(RegisterUserResponseDto? Data, List<(string Type, string Message)>? Errors)> RegisterAsync(RegisterUserRequestDto request);
        Task<(UserProfileResponseDto? Data, List<(string Type, string Message)>? Errors)> GetProfileAsync();
        Task<(UserProfileResponseDto? Data, List<(string Type, string Message)>? Errors)> UpdateProfileAsync(PatchUserRequestDto request);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> DeleteAccountAsync(DeleteAccountRequestDto request);
    }
}
