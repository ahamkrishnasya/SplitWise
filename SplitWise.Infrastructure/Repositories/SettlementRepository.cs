using Microsoft.EntityFrameworkCore;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Services
{
    public class SettlementRepository : ISettlementRepository
    {
        private readonly ApplicationDbContext _context;

        public SettlementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Settlement>? Settlements, List<(string Type, string Message)>? Errors)>
            GetByGroupAsync(int groupId, int userId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == groupId && !g.IsDeleted);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {groupId} does not exist."));
                return (null, errors);
            }

            var groupMember = await _context.GroupMembers.AsNoTracking()
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);

            if (groupMember == null)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var settlements = await _context.Settlements
                .AsNoTracking()
                .Where(s => !s.IsDeleted
                    && s.ExpenseShare.GroupMemberId == groupMember.Id
                    && s.ExpenseShare.Expense.GroupId == groupId)
                .ToListAsync();

            return (settlements, null);
        }

        public async Task<(List<Settlement>? Settlements, List<(string Type, string Message)>? Errors)>
            GetByExpenseShareAsync(int expenseShareId, int userId)
        {
            var errors = new List<(string Type, string Message)>();

            var expenseShare = await _context.ExpenseShares
                .AsNoTracking()
                .Include(es => es.Expense)
                .FirstOrDefaultAsync(es => es.Id == expenseShareId && !es.IsDeleted);

            if (expenseShare == null)
            {
                errors.Add(("NotFound", $"ExpenseShare with id {expenseShareId} does not exist."));
                return (null, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == expenseShare.Expense.GroupId
                    && m.UserId == userId
                    && !m.IsDeleted);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var settlements = await _context.Settlements
                .AsNoTracking()
                .Where(s => s.ExpenseShareId == expenseShareId && !s.IsDeleted)
                .ToListAsync();

            return (settlements, null);
        }

        public async Task<(Settlement? Settlement, List<(string Type, string Message)>? Errors)>
            CreateAsync(int expenseShareId, Settlement settlement, int userId)
        {
            var errors = new List<(string Type, string Message)>();

            var expenseShare = await _context.ExpenseShares
                .Include(es => es.Expense)
                .Include(es => es.Settlements.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync(es => es.Id == expenseShareId && !es.IsDeleted);

            if (expenseShare == null)
            {
                errors.Add(("NotFound", $"ExpenseShare with id {expenseShareId} does not exist."));
                return (null, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == expenseShare.Expense.GroupId
                    && m.UserId == userId
                    && !m.IsDeleted);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var totalPaid = expenseShare.Settlements.Sum(s => s.Amount);
            var remaining = expenseShare.Amount - totalPaid;

            if (remaining <= 0)
            {
                errors.Add(("ValidationError", "This expense share is already fully settled."));
                return (null, errors);
            }

            if (settlement.Amount > remaining)
            {
                errors.Add(("ValidationError", $"Amount exceeds remaining balance of {remaining}."));
                return (null, errors);
            }

            if (settlement.SettlementDate < expenseShare.Expense.ExpenseDate)
            {
                errors.Add(("ValidationError", $"Settlement date cannot be before the expense date ({expenseShare.Expense.ExpenseDate})."));
                return (null, errors);
            }
            settlement.ExpenseShareId = expenseShareId;
            settlement.CreatedAt = DateTime.UtcNow;
            settlement.IsActive = true;

            await _context.Settlements.AddAsync(settlement);
            await _context.SaveChangesAsync();

            return (settlement, null);
        }

        public async Task<(Settlement? Settlement, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int settlementId, decimal? amount, DateOnly? settlementDate, int userId)
        {
            var errors = new List<(string Type, string Message)>();

            var settlement = await _context.Settlements
                .Include(s => s.ExpenseShare)
                    .ThenInclude(es => es.Settlements.Where(s2 => !s2.IsDeleted))
                .Include(s => s.ExpenseShare)
                    .ThenInclude(es => es.Expense)
                .FirstOrDefaultAsync(s => s.Id == settlementId && !s.IsDeleted);

            if (settlement == null)
            {
                errors.Add(("NotFound", $"Settlement with id {settlementId} does not exist."));
                return (null, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == settlement.ExpenseShare.Expense.GroupId
                    && m.UserId == userId
                    && !m.IsDeleted);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            if (amount.HasValue)
            {
                var totalOtherPaid = settlement.ExpenseShare.Settlements
                    .Where(s => s.Id != settlementId)
                    .Sum(s => s.Amount);
                var remaining = settlement.ExpenseShare.Amount - totalOtherPaid;

                if (amount.Value > remaining)
                {
                    errors.Add(("ValidationError", $"Amount exceeds remaining balance of {remaining}."));
                    return (null, errors);
                }

                settlement.Amount = amount.Value;
            }

            if (settlementDate.HasValue)
            {
                if (settlementDate.Value < settlement.ExpenseShare.Expense.ExpenseDate)
                {
                    errors.Add(("ValidationError", $"Settlement date cannot be before the expense date ({settlement.ExpenseShare.Expense.ExpenseDate})."));
                    return (null, errors);
                }

                settlement.SettlementDate = settlementDate.Value;
            }

            settlement.LastModifiedBy = userId;
            settlement.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (settlement, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int settlementId, int userId)
        {
            var errors = new List<(string Type, string Message)>();

            var settlement = await _context.Settlements
                .Include(s => s.ExpenseShare)
                    .ThenInclude(es => es.Expense)
                .FirstOrDefaultAsync(s => s.Id == settlementId && !s.IsDeleted);

            if (settlement == null)
            {
                errors.Add(("NotFound", $"Settlement with id {settlementId} does not exist."));
                return (false, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == settlement.ExpenseShare.Expense.GroupId
                    && m.UserId == userId
                    && !m.IsDeleted);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (false, errors);
            }

            settlement.IsDeleted = true;
            settlement.IsActive = false;
            settlement.LastModifiedAt = DateTime.UtcNow;
            settlement.LastModifiedBy = userId;

            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
