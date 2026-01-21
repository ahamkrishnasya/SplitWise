using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;
using SplitWise.Application.Interfaces.Repositories;

namespace SplitWise.Infrastructure.Services
{
    public class GroupRepository: IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Groups data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Groups>> GetGroupsByUserIdAsync(string userId)
        {
            return _context.Groups.Where(g => g.CreatedByUserId == userId).ToList();
        }   
    }
}
