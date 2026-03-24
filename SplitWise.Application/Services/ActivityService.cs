using SplitWise.Application.DTOs.Activities;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;

namespace SplitWise.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _ActivityRepository;
        private readonly ILoginRepository _loginRepository;

        public ActivityService(IActivityRepository ActivityRepository, ILoginRepository loginRepository)
        {
            _ActivityRepository = ActivityRepository;
            _loginRepository = loginRepository;
        }

        public async Task<List<ActivityResponseDto>> GetActivities()
        {
            var userId = _loginRepository.GetUserId();
            return await _ActivityRepository.GetActivities(userId);
        }
    }
}
