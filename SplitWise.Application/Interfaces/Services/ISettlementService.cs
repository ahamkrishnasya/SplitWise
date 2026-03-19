using SplitWise.Application.DTOs.Settlements;

namespace SplitWise.Application.Interfaces.Services
{
    public interface ISettlementService
    {
        Task<(List<SettlementResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetGroupSettlementsAsync(int groupId);

        Task<(List<SettlementResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetExpenseShareSettlementsAsync(int expenseShareId);

        Task<(SettlementResponseDto? Data, List<(string Type, string Message)>? Errors)>
            CreateSettlementAsync(int expenseShareId, CreateSettlementRequestDto request);

        Task<(SettlementResponseDto? Data, List<(string Type, string Message)>? Errors)>
            UpdateSettlementAsync(int settlementId, PatchSettlementRequestDto request);

        Task<(bool Success, List<(string Type, string Message)>? Errors)>
            DeleteSettlementAsync(int settlementId);
    }
}
