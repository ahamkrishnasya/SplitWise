using SplitWise.Application.DTOs.Groups;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task<List<GroupResponseDto>> Groups();
        Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto Data);
        Task<GroupResponseDto> GetGroupByIdAsync(int id);
        Task<List<GroupMemberResponseDto>> AddMembers(GroupMemberRequestDto request, int id);
        Task EditGroup (int userId);
    }
}
