using SplitWise.Application.DTOs.Sessions;

namespace SplitWise.Application.Interfaces.Services
{
    public interface ISessionService
    {
        Task<SessionResponseDto> Login(SessionRequestDto request);
    }
}
