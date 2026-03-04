namespace SplitWise.Application.Interfaces.Services
{
    public interface IPasswordResetService
    {
        Task<(bool Success, List<(string Type, string Message)>? Errors)> RequestPasswordResetAsync(string email);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> ResetPasswordAsync(string token, string newPasswordBase64);
    }
}
