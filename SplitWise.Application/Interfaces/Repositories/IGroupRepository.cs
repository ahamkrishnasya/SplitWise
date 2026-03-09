using SplitWise.Application.DTOs.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Groups>> Groups(int userId);
        Task<Groups> AddAsync(Groups group);
        Task<Groups> GetByIdAsync(int id);
        Task<bool> IsAdmin(int userId, int groupId);
        Task<bool> CanAdd(GroupMember groupMember);
        Task<List<GroupMember>> AddMembersAsync(List<GroupMember> groupMembers);
        Task<Groups> EditGroup(Groups group);
        Task<GroupMember> RemoveMemberAsync(int groupId, int userId);
        //Task<List<GroupMember>> AddMembers(int id, int userId);
    }
}
