using SplitWise.Application.DTOs.EmailVerification;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IEmailVerificationService
    {
        Task<(VerifyEmailResponseDto? Data, List<(string Type, string Message)>? Errors)> VerifyEmailAsync(string token);
    }
}
