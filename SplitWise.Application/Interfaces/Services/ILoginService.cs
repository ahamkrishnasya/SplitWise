using SplitWise.Application.DTOs.Sessions;

namespace SplitWise.Application.Interfaces.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDto> Login(LoginRequestDto request);
    }
}
