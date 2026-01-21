using SplitWise.Application.DTOs.Groups;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task CreateGroupAsync(CreateGroupDto Data);
        Task<IEnumerable<CreateGroupDto>> GetGroupsByUserIdAsync(string userId);
        Task EditGroup (string userId);
    }
}
