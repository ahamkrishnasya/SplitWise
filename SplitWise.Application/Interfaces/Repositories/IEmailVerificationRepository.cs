using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IEmailVerificationRepository
    {
        Task AddAsync(EmailVerificationToken token);
        Task<EmailVerificationToken?> GetByTokenAsync(string token);
        Task SaveChangesAsync();
    }
}
