using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface ISettlementRepository
    {
        Task<(List<Settlement>? Settlements, List<(string Type, string Message)>? Errors)>
            GetByGroupAsync(int groupId, int userId);

        Task<(List<Settlement>? Settlements, List<(string Type, string Message)>? Errors)>
            GetByExpenseShareAsync(int expenseShareId, int userId);

        Task<(Settlement? Settlement, List<(string Type, string Message)>? Errors)>
            CreateAsync(int expenseShareId, Settlement settlement, int userId);

        Task<(Settlement? Settlement, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int settlementId, decimal? amount, DateOnly? settlementDate, int userId);

        Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int settlementId, int userId);
    }
}

