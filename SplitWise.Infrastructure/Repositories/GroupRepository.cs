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
using SplitWise.Application.DTOs.Groups;

namespace SplitWise.Infrastructure.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupResponseDto>> Groups(int userId)
        {
            return await (
                from g in _context.Groups
                join gm in _context.GroupMembers on g.Id equals gm.GroupId
                join u in _context.Users on g.CreatedBy equals u.Id
                where gm.UserId == userId && gm.IsActive == true && g.IsActive == true
                select new GroupResponseDto
                {
                    GroupId = g.Id,
                    GroupName = g.GroupName,
                    Description = g.Description,
                    CreatedByUserId = g.CreatedBy,
                    CreatedByFirstName = u.FirstName,
                    CreatedByLastName = u.LastName,
                    CreatedByEmail = u.Email,
                    CreatedAt = g.CreatedAt,
                    LastModifiedAt = g.LastModifiedAt
                }
            )
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAt)
            .Distinct()
            .ToListAsync();
        }

        public async Task<GroupResponseDto?> GetGroupDetailAsync(int id)
        {
            return await (
                from g in _context.Groups
                join u in _context.Users on g.CreatedBy equals u.Id
                where g.Id == id
                select new GroupResponseDto
                {
                    GroupId = g.Id,
                    GroupName = g.GroupName,
                    Description = g.Description,
                    CreatedByUserId = g.CreatedBy,
                    CreatedByFirstName = u.FirstName,
                    CreatedByLastName = u.LastName,
                    CreatedByEmail = u.Email,
                    CreatedAt = g.CreatedAt,
                    LastModifiedAt = g.LastModifiedAt
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync();
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

        public async Task<List<GroupMemberResponseDto>> GetMembers(int groupId)
        {
            return await (
                from gm in _context.GroupMembers
                join u in _context.Users on gm.UserId equals u.Id
                join cu in _context.Users on gm.CreatedBy equals cu.Id into creatorJoin
                from cu in creatorJoin.DefaultIfEmpty()
                where gm.GroupId == groupId && gm.IsActive == true
                      && u.IsActive == true && !u.IsDeleted
                select new GroupMemberResponseDto
                {
                    Id = gm.Id,
                    GroupId = gm.GroupId,
                    MemberId = gm.UserId,
                    CreatedByUserId = gm.CreatedBy,
                    CreatedByFirstName = cu != null ? cu.FirstName : null,
                    CreatedByLastName = cu != null ? cu.LastName : null,
                    CreatedByEmail = cu != null ? cu.Email : null,
                    IsAdmin = gm.IsAdmin,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }
            ).AsNoTracking().ToListAsync();
        }

        public async Task<List<GroupMemberResponseDto>> NotInGroup(int groupId, int userId)
        {
            return await (
                from f in _context.Friendships
                where (f.UserId1 == userId || f.UserId2 == userId)
                      && f.IsActive == true
                join u in _context.Users
                    on (f.UserId1 == userId ? f.UserId2 : f.UserId1) equals u.Id
                where u.IsActive == true && !u.IsDeleted
                      && !_context.GroupMembers
                            .Any(gm => gm.GroupId == groupId && gm.UserId == u.Id)
                select new GroupMemberResponseDto
                {
                    MemberId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                }
            )
            .AsNoTracking()
            .ToListAsync();
        }
        public async Task<GroupMember?> TransferAdminAsync(int groupId, int fromUserId, int toUserId)
        {
            var newAdminMember = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == toUserId
                                       && m.IsActive == true && m.IsDeleted == false);

            if (newAdminMember == null)
                return null;

            var currentAdminMember = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == fromUserId && m.IsAdmin == true);

            if (currentAdminMember != null)
            {
                currentAdminMember.IsAdmin = false;
                currentAdminMember.LastModifiedAt = DateTime.UtcNow;
                currentAdminMember.LastModifiedBy = fromUserId;
            }

            newAdminMember.IsAdmin = true;
            newAdminMember.LastModifiedAt = DateTime.UtcNow;
            newAdminMember.LastModifiedBy = fromUserId;

            await _context.SaveChangesAsync();
            return newAdminMember;
        }
    }
}
