using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IExpenseRepository
    {
        Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            CreateAsync(Expense expense, List<ExpenseShare> shares, int currentUserId);

        Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int groupId, int expenseId, Expense updatedExpense, List<ExpenseShare> newShares, int currentUserId);

        Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            GetByIdAsync(int groupId, int expenseId, int currentUserId);

        Task<(List<Expense>? Expenses, List<(string Type, string Message)>? Errors)>
            GetAllByGroupAsync(int groupId, int currentUserId);

        Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int groupId, int expenseId, int currentUserId);
    }
}
