using SplitWise.Application.DTOs.Expenses;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILoginRepository _loginRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ILoginRepository loginRepository)
        {
            _expenseRepository = expenseRepository;
            _loginRepository = loginRepository;
        }

        public async Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            CreateAsync(int groupId, CreateExpenseRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();

            if (request.TotalAmount <= 0)
                errors.Add(("ValidationError", "TotalAmount must be greater than 0."));

            if (request.Shares == null || request.Shares.Count == 0)
                errors.Add(("ValidationError", "Shares must not be empty."));

            if (errors.Count > 0)
                return (null, errors);

            var hasDuplicateMembers = request.Shares
                .GroupBy(s => s.GroupMemberId)
                .Any(g => g.Count() > 1);

            if (hasDuplicateMembers)
                errors.Add(("ValidationError", "Shares contain duplicate GroupMemberIds."));

            var invalidAmounts = request.Shares.Where(s => s.Amount < 0).ToList();
            if (invalidAmounts.Any())
                errors.Add(("ValidationError", "Share amounts must be >= 0."));

            var sharesSum = request.Shares.Sum(s => s.Amount);
            if (sharesSum != request.TotalAmount)
                errors.Add(("ValidationError", $"The sum of share amounts ({sharesSum}) must equal TotalAmount ({request.TotalAmount})."));

            if (errors.Count > 0)
                return (null, errors);

            var currentUserId = _loginRepository.GetUserId();

            var expense = new Expense
            {
                GroupId = groupId,
                Description = request.Description,
                TotalAmount = request.TotalAmount,
                PaidByGroupMemberId = request.PaidByGroupMemberId,
                ExpenseDate = request.ExpenseDate,
                CreatedBy = currentUserId
            };

            var shares = request.Shares.Select(s => new ExpenseShare
            {
                GroupMemberId = s.GroupMemberId,
                Amount = s.Amount
            }).ToList();

            var (createdExpense, repoErrors) = await _expenseRepository.CreateAsync(expense, shares, currentUserId);

            if (repoErrors != null && repoErrors.Count > 0)
                return (null, repoErrors);

            var response = new CreateExpenseResponseDto
            {
                Id = createdExpense!.Id,
                GroupId = createdExpense.GroupId,
                Description = createdExpense.Description,
                TotalAmount = createdExpense.TotalAmount,
                PaidByGroupMemberId = createdExpense.PaidByGroupMemberId,
                ExpenseDate = createdExpense.ExpenseDate,
                CreatedAt = createdExpense.CreatedAt,
                Shares = shares.Select(s => new ExpenseShareResponseDto
                {
                    Id = s.Id,
                    GroupMemberId = s.GroupMemberId,
                    Amount = s.Amount
                }).ToList()
            };

            return (response, null);
        }

        public async Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int groupId, int expenseId, CreateExpenseRequestDto request)
        {
            var errors = new List<(string Type, string Message)>();

            if (request.TotalAmount <= 0)
                errors.Add(("ValidationError", "TotalAmount must be greater than 0."));

            if (request.Shares == null || request.Shares.Count == 0)
                errors.Add(("ValidationError", "Shares must not be empty."));

            if (errors.Count > 0)
                return (null, errors);

            var hasDuplicateMembers = request.Shares
                .GroupBy(s => s.GroupMemberId)
                .Any(g => g.Count() > 1);

            if (hasDuplicateMembers)
                errors.Add(("ValidationError", "Shares contain duplicate GroupMemberIds."));

            if (request.Shares.Any(s => s.Amount < 0))
                errors.Add(("ValidationError", "Share amounts must be >= 0."));

            var sharesSum = request.Shares.Sum(s => s.Amount);
            if (sharesSum != request.TotalAmount)
                errors.Add(("ValidationError", $"The sum of share amounts ({sharesSum}) must equal TotalAmount ({request.TotalAmount})."));

            if (errors.Count > 0)
                return (null, errors);

            var currentUserId = _loginRepository.GetUserId();

            var updatedExpense = new Expense
            {
                GroupId = groupId,
                Description = request.Description,
                TotalAmount = request.TotalAmount,
                PaidByGroupMemberId = request.PaidByGroupMemberId,
                ExpenseDate = request.ExpenseDate
            };

            var newShares = request.Shares.Select(s => new ExpenseShare
            {
                GroupMemberId = s.GroupMemberId,
                Amount = s.Amount
            }).ToList();

            var (expense, repoErrors) = await _expenseRepository.UpdateAsync(groupId, expenseId, updatedExpense, newShares, currentUserId);

            if (repoErrors != null && repoErrors.Count > 0)
                return (null, repoErrors);

            var response = new CreateExpenseResponseDto
            {
                Id = expense!.Id,
                GroupId = expense.GroupId,
                Description = expense.Description,
                TotalAmount = expense.TotalAmount,
                PaidByGroupMemberId = expense.PaidByGroupMemberId,
                ExpenseDate = expense.ExpenseDate,
                CreatedAt = expense.CreatedAt,
                Shares = expense.Shares.Select(s => new ExpenseShareResponseDto
                {
                    Id = s.Id,
                    GroupMemberId = s.GroupMemberId,
                    Amount = s.Amount
                }).ToList()
            };

            return (response, null);
        }

        public async Task<(CreateExpenseResponseDto? Data, List<(string Type, string Message)>? Errors)>
            GetByIdAsync(int groupId, int expenseId)
        {
            var currentUserId = _loginRepository.GetUserId();
            var (expense, errors) = await _expenseRepository.GetByIdAsync(groupId, expenseId, currentUserId);

            if (errors != null && errors.Count > 0)
                return (null, errors);

            var response = new CreateExpenseResponseDto
            {
                Id = expense!.Id,
                GroupId = expense.GroupId,
                Description = expense.Description,
                TotalAmount = expense.TotalAmount,
                PaidByGroupMemberId = expense.PaidByGroupMemberId,
                ExpenseDate = expense.ExpenseDate,
                CreatedAt = expense.CreatedAt,
                Shares = expense.Shares.Select(s => new ExpenseShareResponseDto
                {
                    Id = s.Id,
                    GroupMemberId = s.GroupMemberId,
                    Amount = s.Amount
                }).ToList()
            };

            return (response, null);
        }

        public async Task<(List<CreateExpenseResponseDto>? Data, List<(string Type, string Message)>? Errors)>
            GetAllByGroupAsync(int groupId)
        {
            var currentUserId = _loginRepository.GetUserId();
            var (expenses, errors) = await _expenseRepository.GetAllByGroupAsync(groupId, currentUserId);

            if (errors != null && errors.Count > 0)
                return (null, errors);

            var response = expenses!.Select(expense => new CreateExpenseResponseDto
            {
                Id = expense.Id,
                GroupId = expense.GroupId,
                Description = expense.Description,
                TotalAmount = expense.TotalAmount,
                PaidByGroupMemberId = expense.PaidByGroupMemberId,
                ExpenseDate = expense.ExpenseDate,
                CreatedAt = expense.CreatedAt,
                Shares = expense.Shares.Select(s => new ExpenseShareResponseDto
                {
                    Id = s.Id,
                    GroupMemberId = s.GroupMemberId,
                    Amount = s.Amount
                }).ToList()
            }).ToList();

            return (response, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int groupId, int expenseId)
        {
            var currentUserId = _loginRepository.GetUserId();
            return await _expenseRepository.SoftDeleteAsync(groupId, expenseId, currentUserId);
        }
    }
}
