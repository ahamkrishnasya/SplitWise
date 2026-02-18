using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;
using SplitWise.Application.Interfaces.Repositories;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SplitWise.Infrastructure.Services
{
    public class GroupRepository: IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Groups> AddAsync(Groups data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();

            var groupMembers = new GroupMember
            {
                GroupId = data.Id,
                UserId = data.CreatedBy,
                IsAdmin = true,
                CreatedBy = data.CreatedBy,
            };

            _context.Add(groupMembers);
            await _context.SaveChangesAsync();

            return(data);
        }

        public async Task<Groups> GetByIdAsync(int id)
        {
            return await _context.Groups.Where(x => x.Id == id).FirstOrDefaultAsync();
        }   

        //public async Task<GroupMember> AddMembersAsync(GroupMember groupMembers)
        //{
        //    _context.AddRangeAsync(groupMembers);
        //    await _context.SaveChangesAsync();
        //    return (groupMembers);
        //}
    }
}
