using SplitWise.Application.DTOs.Activities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IActivityRepository
    {
        Task<List<ActivityResponseDto>> GetActivities(int userId);
    }
}
