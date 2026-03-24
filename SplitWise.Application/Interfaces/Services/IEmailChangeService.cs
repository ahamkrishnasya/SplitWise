using SplitWise.Application.DTOs.EmailChange;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IEmailChangeService
    {
        Task<(bool Success, List<(string Type, string Message)>? Errors)> RequestEmailChangeAsync(RequestEmailChangeDto request);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> ConfirmEmailChangeAsync(ConfirmEmailChangeDto request);
    }
}
