using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IPasswordResetRepository
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task DeleteTokensByUserIdAsync(int userId);
        Task AddAsync(PasswordResetToken token);
        void Delete(PasswordResetToken token);
        Task SaveChangesAsync();
    }
}
