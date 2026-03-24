using Microsoft.EntityFrameworkCore;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<(User? User, List<(string Type, string Message)>? Errors)> UpdateNameAsync(
            int userId, string? firstName, string? lastName)
        {
            var errors = new List<(string Type, string Message)>();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
            {
                errors.Add(("NotFound", "User not found."));
                return (null, errors);
            }

            if (firstName is not null)
                user.FirstName = firstName;

            if (lastName is not null)
                user.LastName = lastName;

            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = userId;

            await _context.SaveChangesAsync();

            return (user, null);
        }
        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> UpdatePasswordAsync(
    int userId, string newPasswordHash)
        {
            var errors = new List<(string Type, string Message)>();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
            {
                errors.Add(("NotFound", "User not found."));
                return (false, errors);
            }

            user.PasswordHash = newPasswordHash;
            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = userId;

            await _context.SaveChangesAsync();

            return (true, null);
        }
        public async Task<(bool Success, List<(string Type, string Message)>? Errors)> DeleteAccountAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return (false, new List<(string, string)> { ("NotFound", "User not found.") });

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            user.Email = $"{user.Email}_deleted_{timestamp}";
            user.IsDeleted = true;
            user.IsActive = false;
            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = userId;

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
