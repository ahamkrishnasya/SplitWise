using SplitWise.Application.DTOs.Expenses;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            CreateAsync(int groupId, CreateExpenseRequestDto request);

        Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int groupId, int expenseId, CreateExpenseRequestDto request);

        Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            GetByIdAsync(int groupId, int expenseId);

        Task<(List<CreateExpenseResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetAllByGroupAsync(int groupId);

        Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int groupId, int expenseId);
    }
}
