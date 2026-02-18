using SplitWise.Application.DTOs.Groups;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto Data);
        Task<GroupResponseDto> GetGroupByIdAsync(int id);
        //Task<GroupMemberResponseDto> AddMembers(GroupMemberRequestDto request);
        Task EditGroup (int userId);
    }
}
