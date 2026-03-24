using Microsoft.EntityFrameworkCore;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Services
{
    public class EmailChangeRepository : IEmailChangeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailChangeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailChangeToken token)
        {
            await _context.EmailChangeTokens.AddAsync(token);
        }

        public async Task<EmailChangeToken?> GetByTokenAsync(string token)
        {
            return await _context.EmailChangeTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var tokens = await _context.EmailChangeTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.EmailChangeTokens.RemoveRange(tokens);
        }

        public void Delete(EmailChangeToken token)
        {
            _context.EmailChangeTokens.Remove(token);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
