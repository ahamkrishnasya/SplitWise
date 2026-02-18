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
        Task<Groups> AddAsync(Groups group);
        Task<Groups> GetByIdAsync(int id);   
        //Task<List<GroupMember>> AddMembersAsync(GroupMember groupMembers);
    }
}
