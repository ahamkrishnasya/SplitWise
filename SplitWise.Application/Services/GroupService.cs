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

namespace SplitWise.Application.Services
{
    public class GroupService: IGroupService
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
                GroupName = x.GroupName
            }).ToList();
        }
        public async Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto Data)
        {
            var group = new Groups
            {
                GroupName = Data.GroupName,
                Description = Data.Description,
                CreatedBy = _loginRepository.GetUserId()
            };
            var result = await _groupRepository.AddAsync(group);

            return new GroupResponseDto
            {
                GroupId = result.Id,
                GroupName = result.GroupName,
                Description = result.Description,
                CreatedByUserId = result.CreatedBy
            };
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
                if(userid == memberId)
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

            if(groupMembers != null)
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
        public async Task EditGroup (int userId)
        {
            
            await Task.CompletedTask;
        }
    }
}
