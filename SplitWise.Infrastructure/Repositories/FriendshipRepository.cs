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
using SplitWise.Application.DTOs.Friendships;

namespace SplitWise.Infrastructure.Services
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _context;
        public FriendshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsExist(Friendship friend)
        {
            return await _context.Friendships.AnyAsync(f => f.UserId1 == friend.UserId1 && f.UserId2 == friend.UserId2);
        }   
        public async Task<List<Friendship>> AddAsync(List<Friendship> data)
        {
            await _context.AddRangeAsync(data);
            await _context.SaveChangesAsync();
            return (data);
        }

        public async Task<List<FriendshipResponseDto>> GetAsync(int userId)
        {
            return await _context.Friendships
                .AsNoTracking()
                .Where(f => f.UserId1 == userId || f.UserId2 == userId)
                .Select(f => new FriendshipResponseDto
                {
                    Id = f.Id,
                    FriendUserId = f.UserId1 == userId ? f.UserId2 : f.UserId1,
                    CreatedByUserId = f.CreatedBy,
                    FirstName = f.UserId1 == userId ? f.User2.FirstName : f.User1.FirstName,
                    LastName = f.UserId1 == userId ? f.User2.LastName : f.User1.LastName
                }).ToListAsync();
        }

        public async Task<List<User>> NotInFriends(int userId)
        {
            return await _context.Users
                .Where(u => u.Id != userId &&
                !_context.Friendships.Any(f =>
                    (f.UserId1 == userId && f.UserId2 == u.Id) ||
                    (f.UserId2 == userId && f.UserId1 == u.Id)))
                .ToListAsync();
        }

    }
}
