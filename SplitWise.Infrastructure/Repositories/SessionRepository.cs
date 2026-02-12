using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Services
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> Login(User request)
        {
            var user = await _context.Users.Where(u => u.Email == request.Email).FirstOrDefaultAsync();
            return user;
        }
    }
}
