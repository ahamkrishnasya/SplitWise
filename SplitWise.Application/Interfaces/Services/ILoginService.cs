using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Sessions;

namespace SplitWise.Application.Interfaces.Services
{
    public interface ILoginService
    {
        Task<ApiResponse<object>> Login(LoginRequestDto request);
    }
}
