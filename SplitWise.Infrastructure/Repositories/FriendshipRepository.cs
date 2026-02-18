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

        public async Task<bool> IsExist(int user1, int user2)
        {
            return await _context.Friendships.AnyAsync(f => f.UserId1 == user1 && f.UserId2 == user2);
        }   
        public async Task<Friendship> AddAsync(Friendship data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
            return (data);
        }

        public async Task<List<Friendship>> GetAsync(int userid)
        {
            return await _context.Friendships.Where(x => x.UserId1 == userid || x.UserId2 == userid).ToListAsync();
        }

    }
}
