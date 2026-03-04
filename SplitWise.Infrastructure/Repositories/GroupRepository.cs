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
using Microsoft.AspNetCore.Http.HttpResults;

namespace SplitWise.Infrastructure.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Groups>> Groups(int userId)
        {
            return await _context.Groups.AsNoTracking().
                Where(x => x.CreatedBy == userId && x.IsActive == true).ToListAsync();
        }
        public async Task<Groups> AddAsync(Groups data)
        {
            var exist = await _context.Groups.AsNoTracking().
                Where(x => x.GroupName == data.GroupName && x.CreatedBy == data.CreatedBy).
                AnyAsync();

            if (!exist)
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

                return (data);
            }
            return null;
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

            if (user)
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

        //public async Task<List<GroupMember>> AddMembers(int id, int userId)
        //{
        //    var friendsNotInGroup =  await _context.Friendships(x=> x.UserId1 == userId || x.UserId2 == userId)
        //}
        public async Task<List<GroupMember>> AddMembersAsync(List<GroupMember> groupMembers)
        {
            await _context.AddRangeAsync(groupMembers);
            await _context.SaveChangesAsync();
            return (groupMembers);
        }

        public async Task<Groups> EditGroup(Groups group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }
        public async Task<GroupMember> RemoveMemberAsync(int groupId, int userId)
        {
            var member = await _context.GroupMembers.Where(x => x.GroupId == groupId && x.UserId == userId).FirstOrDefaultAsync();
            if (member != null)
            {
                member.IsDeleted = true;
                member.IsActive = false;
                _context.GroupMembers.Update(member);
                await _context.SaveChangesAsync();
                return member;
            }
            return null;
        }
    }
}
