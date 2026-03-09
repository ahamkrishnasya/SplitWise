using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using SplitWise.Application.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace SplitWise.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILoginRepository _loginRepository;

        public GroupService(IGroupRepository groupRepository, ILoginRepository loginRepository)
        {
            _groupRepository = groupRepository;
            _loginRepository = loginRepository;
        }

        public async Task<List<GroupResponseDto>> Groups()
        {
            var userId = _loginRepository.GetUserId();
            var groups = await _groupRepository.Groups(userId);

            return groups.Select(x => new GroupResponseDto
            {
                GroupId = x.Id,
                GroupName = x.GroupName,
                Description = x.Description
            }).ToList();
        }
        public async Task<ApiResponse<object>> CreateGroupAsync(GroupRequestDto Data)
        {
            var group = new Groups
            {
                GroupName = Data.GroupName,
                Description = Data.Description,
                CreatedBy = _loginRepository.GetUserId()
            };
            var result = await _groupRepository.AddAsync(group);

            if (result == null)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Group creation failed",
                    errorType: "Duplicate_Group",
                    errorMessage: "Group with same name is already existed",
                    statusCode: StatusCodes.Status409Conflict
                );
            }
            return ApiResponseFactory.Success<object>(
                data: new
                {
                    id = result.Id,
                    groupName = result.GroupName,
                    description = result.Description,
                    createdBy = result.CreatedBy
                },
                message: "Group created successfully",
                statusCode: StatusCodes.Status201Created
            );
        }

        public async Task<GroupResponseDto> GetGroupByIdAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            var result = new GroupResponseDto
            {
                GroupId = group.Id,
                GroupName = group.GroupName,
                Description = group.Description,
                CreatedByUserId = group.CreatedBy
            };
            return result;
        }

        //public async Task<GroupMemberRequestDto> AddMembers(int id)
        //{
        //    var userId = _loginRepository.GetUserId();
        //    var newMembers = await _groupRepository.AddMembers(id, userId);
        //}

        public async Task<List<GroupMemberResponseDto>> AddMembers(GroupMemberRequestDto request, int id)
        {
            var userid = _loginRepository.GetUserId();

            bool admin = await _groupRepository.IsAdmin(userid, id);

            if (!admin)
            {
                return null;
            }

            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (var memberId in request.MemberId)
            {
                if (userid == memberId)
                {
                    continue;
                }
                var groupMember = new GroupMember
                {
                    GroupId = id,
                    UserId = memberId,
                    CreatedBy = userid,
                    IsAdmin = false
                };
                bool canAdd = await _groupRepository.CanAdd(groupMember);
                if (!canAdd)
                {
                    continue;
                }
                groupMembers.Add(groupMember);
            }

            if (groupMembers != null)
            {
                var result = await _groupRepository.AddMembersAsync(groupMembers);

                return result.Select(x => new GroupMemberResponseDto
                {
                    Id = x.Id,
                    GroupId = x.GroupId,
                    MemberId = x.UserId,
                    CreatedByUserId = x.CreatedBy,
                    IsAdmin = x.IsAdmin,
                }).ToList();
            }
            return null;
        }

        public async Task<ApiResponse<object>> EditGroup(GroupRequestDto request, int id)
        {
            var userid = _loginRepository.GetUserId();
            bool admin = await _groupRepository.IsAdmin(userid, id);
            if (!admin)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Unauthorized to edit group",
                    errorType: "Unauthorized",
                    errorMessage: "Only group admins can edit the group",
                    statusCode: StatusCodes.Status403Forbidden
                );
            }

            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Group not found",
                    errorType: "Not_Found",
                    errorMessage: $"No group found with id {id}",
                    statusCode: StatusCodes.Status404NotFound
                );
            }
            group.GroupName = request.GroupName;
            group.Description = request.Description;
            group.LastModifiedBy = userid;
            group.LastModifiedAt = DateTime.UtcNow;

            var result = await _groupRepository.EditGroup(group);

            return ApiResponseFactory.Success<object>(
                data: result,
                message: "Group updated successfully",
                statusCode: StatusCodes.Status200OK
            );
        }

        public async Task<ApiResponse<object>> DeleteGroup(int id)
        {
            var userid = _loginRepository.GetUserId();
            bool admin = await _groupRepository.IsAdmin(userid, id);
            if (!admin)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Unauthorized to delete group",
                    errorType: "Unauthorized",
                    errorMessage: "Only group admins can delete the group",
                    statusCode: StatusCodes.Status403Forbidden
                );
            }
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Group not found",
                    errorType: "Not_Found",
                    errorMessage: $"No group found with id {id}",
                    statusCode: StatusCodes.Status404NotFound
                );
            }
            group.IsDeleted = true;
            group.IsActive = false;
            group.LastModifiedAt = DateTime.UtcNow;
            group.LastModifiedBy = userid;
            var result = await _groupRepository.EditGroup(group);
            return ApiResponseFactory.Success<object>(
                data: result,
                message: "Group deleted successfully",
                statusCode: StatusCodes.Status200OK
            );
        }

        public async Task<ApiResponse<object>> RemoveMember(int groupId, int memberId)
        {
            var userid = _loginRepository.GetUserId();
            bool admin = await _groupRepository.IsAdmin(userid, groupId);
            if (!admin)
            {
                return ApiResponseFactory.Failure<object>(
                    message: "Unauthorized to remove members",
                    errorType: "Unauthorized",
                    errorMessage: "Only group admins can remove members from the group",
                    statusCode: StatusCodes.Status403Forbidden
                );
            }
            
            var result = await _groupRepository.RemoveMemberAsync(groupId, memberId); 

            if(result == null)
            {
                return ApiResponseFactory.Failure<object>(
                    message : "User not found",
                    errorType : "Not_Found",
                    errorMessage : "User doesn't belongs to group",
                    statusCode : StatusCodes.Status404NotFound
                );
            }
            return ApiResponseFactory.Success<object>(
                data: memberId,
                message: "Members removed successfully",
                statusCode: StatusCodes.Status200OK
            );
        }

        public async Task<List<GroupMemberResponseDto>> GetMembers(int groupId)
        {
            return await _groupRepository.GetMembers(groupId);
        }

        public async Task<List<GroupMemberResponseDto>> NotInGroup(int groupId)
        {
            var userId = _loginRepository.GetUserId();

            return await _groupRepository.NotInGroup(groupId, userId);
        }
    }
}
