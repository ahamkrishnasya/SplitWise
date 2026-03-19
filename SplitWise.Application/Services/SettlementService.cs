using SplitWise.Application.DTOs.Settlements;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class SettlementService : ISettlementService
    {
        private readonly ISettlementRepository _settlementRepository;
        private readonly ILoginRepository _loginRepository;

        public SettlementService(ISettlementRepository settlementRepository, ILoginRepository loginRepository)
        {
            _settlementRepository = settlementRepository;
            _loginRepository = loginRepository;
        }

        public async Task<(List<SettlementResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetGroupSettlementsAsync(int groupId)
        {
            var userId = _loginRepository.GetUserId();
            var (settlements, errors) = await _settlementRepository.GetByGroupAsync(groupId, userId);

            if (errors != null && errors.Count > 0)
                return (null, errors);

            var response = settlements!.Select(s => new SettlementResponseDto
            {
                Id = s.Id,
                ExpenseShareId = s.ExpenseShareId,
                Amount = s.Amount,
                SettlementDate = s.SettlementDate,
                CreatedAt = s.CreatedAt
            }).ToList();

            return (response, null);
        }

        public async Task<(List<SettlementResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetExpenseShareSettlementsAsync(int expenseShareId)
        {
            var userId = _loginRepository.GetUserId();
            var (settlements, errors) = await _settlementRepository.GetByExpenseShareAsync(expenseShareId, userId);

            if (errors != null && errors.Count > 0)
                return (null, errors);

            var response = settlements!.Select(s => new SettlementResponseDto
            {
                Id = s.Id,
                ExpenseShareId = s.ExpenseShareId,
                Amount = s.Amount,
                SettlementDate = s.SettlementDate,
                CreatedAt = s.CreatedAt
            }).ToList();

            return (response, null);
        }

        public async Task<(SettlementResponseDto? Data, List<(string Type, string Message)>? Errors)>
            CreateSettlementAsync(int expenseShareId, CreateSettlementRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();

            if (request.Amount <= 0)
            {
                errors.Add(("ValidationError", "Amount must be greater than 0."));
                return (null, errors);
            }

            var userId = _loginRepository.GetUserId();

            var settlement = new Settlement
            {
                ExpenseShareId = expenseShareId,
                Amount = request.Amount,
                SettlementDate = request.SettlementDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedBy = userId
            };

            var (created, repoErrors) = await _settlementRepository.CreateAsync(expenseShareId, settlement, userId);

            if (repoErrors != null && repoErrors.Count > 0)
                return (null, repoErrors);

            var response = new SettlementResponseDto
            {
                Id = created!.Id,
                ExpenseShareId = created.ExpenseShareId,
                Amount = created.Amount,
                SettlementDate = created.SettlementDate,
                CreatedAt = created.CreatedAt
            };

            return (response, null);
        }

        public async Task<(SettlementResponseDto? Data, List<(string Type, string Message)>? Errors)>
            UpdateSettlementAsync(int settlementId, PatchSettlementRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();

            if (request.Amount.HasValue && request.Amount.Value <= 0)
            {
                errors.Add(("ValidationError", "Amount must be greater than 0."));
                return (null, errors);
            }

            var userId = _loginRepository.GetUserId();
            var (updated, repoErrors) = await _settlementRepository.UpdateAsync(
                settlementId, request.Amount, request.SettlementDate, userId);

            if (repoErrors != null && repoErrors.Count > 0)
                return (null, repoErrors);

            var response = new SettlementResponseDto
            {
                Id = updated!.Id,
                ExpenseShareId = updated.ExpenseShareId,
                Amount = updated.Amount,
                SettlementDate = updated.SettlementDate,
                CreatedAt = updated.CreatedAt
            };

            return (response, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)>
            DeleteSettlementAsync(int settlementId)
        {
            var userId = _loginRepository.GetUserId();
            return await _settlementRepository.SoftDeleteAsync(settlementId, userId);
        }
    }
}
