using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Groups;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task<List<GroupResponseDto>> Groups();
        Task<ApiResponse<object>> CreateGroupAsync(GroupRequestDto Data);
        Task<GroupResponseDto> GetGroupByIdAsync(int id);
        Task<List<GroupMemberResponseDto>> AddMembers(GroupMemberRequestDto request, int id);
        Task<ApiResponse<object>> EditGroup(GroupRequestDto request, int id);
        Task<ApiResponse<object>> DeleteGroup(int id);
        Task<ApiResponse<object>> RemoveMember(int groupId, int memberid);
        Task<List<GroupMemberResponseDto>> GetMembers(int groupId);
        Task<List<GroupMemberResponseDto>> NotInGroup(int groupId);
    }
}
