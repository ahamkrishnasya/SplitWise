using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class GroupService: IGroupService
    {
        private readonly IGroupRepository _groupRepository; 

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }   

        public async Task CreateGroupAsync(CreateGroupDto Data)
        {
            var group = new Groups
            {
                GroupName = Data.GroupName,
                Description = Data.Description,
                CreatedByUserId = Data.CreatedByUserId
            };
            
            await _groupRepository.AddAsync(group);
        }

        public async Task<IEnumerable<CreateGroupDto>> GetGroupsByUserIdAsync(int userId)
        {
            var groups = await _groupRepository.GetGroupsByUserIdAsync(userId);
            var groupDtos = groups.Select(g => new CreateGroupDto
            {
                GroupName = g.GroupName,
                Description = g.Description,
                CreatedByUserId = g.CreatedByUserId
            });
            return groupDtos;
        }

        public async Task EditGroup (int userId)
        {
            
            await Task.CompletedTask;
        }
    }
}
