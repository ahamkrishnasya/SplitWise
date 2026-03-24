using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IEmailChangeRepository
    {
        Task AddAsync(EmailChangeToken token);
        Task<EmailChangeToken?> GetByTokenAsync(string token);
        Task DeleteByUserIdAsync(int userId);
        void Delete(EmailChangeToken token);
        Task SaveChangesAsync();
    }
}
