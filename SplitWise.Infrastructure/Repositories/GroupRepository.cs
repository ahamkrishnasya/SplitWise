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

        public async Task<List<Groups>> Groups(int userId)
        {
            return await _context.Groups.AsNoTracking().
                Where(x => x.CreatedBy == userId).ToListAsync();
        }
        public async Task<Groups> AddAsync(Groups data)
        {
            await _context.AddAsync(data);
            await _context.SaveChangesAsync();

            var groupMembers = new GroupMember
            {
                GroupId = data.Id,
                UserId = data.CreatedBy,
                IsAdmin = true,
                CreatedBy = data.CreatedBy,
            };

            await _context.AddAsync(groupMembers);
            await _context.SaveChangesAsync();

            return(data);
        }

        public async Task<Groups> GetByIdAsync(int id)
        {
            return await _context.Groups.AsNoTracking().
                Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> IsAdmin(int userId, int groupId)
        {
            return await _context.GroupMembers.AsNoTracking().
                Where(x => x.GroupId == groupId && x.UserId == userId && x.IsAdmin == true).AnyAsync();
        }

        public async Task<bool> CanAdd(GroupMember groupMember)
        {
            var user = await _context.Users.AsNoTracking().Where(x => x.Id == groupMember.UserId).AnyAsync();

            if(user)
            {
                var member = await _context.GroupMembers.AsNoTracking().
                    Where(x => x.GroupId == groupMember.GroupId && x.UserId == groupMember.UserId).AnyAsync();

                return !member;
            }
            else
            {
                return user;
            }
            
        }
        public async Task<List<GroupMember>> AddMembersAsync(List<GroupMember> groupMembers)
        {
            await _context.AddRangeAsync(groupMembers);
            await _context.SaveChangesAsync();
            return (groupMembers);
        }
    }
}
