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

        public async Task<List<Friendship>> GetAsync(int userid)
        {
            return await _context.Friendships.AsNoTracking().Where(x => x.UserId1 == userid || x.UserId2 == userid).ToListAsync();
        }

    }
}
