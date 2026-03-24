using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<User?> GetUserById(int id);
        Task<(User? User, List<(string Type, string Message)>? Errors)> UpdateNameAsync(int userId, string? firstName, string? lastName);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<(bool Success, List<(string Type, string Message)>? Errors)> DeleteAccountAsync(int userId);
    }
}


