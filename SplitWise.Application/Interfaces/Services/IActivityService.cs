using SplitWise.Application.DTOs.Activities;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IActivityService
    {
        public Task<List<ActivityResponseDto>> GetActivities();
    }
}
