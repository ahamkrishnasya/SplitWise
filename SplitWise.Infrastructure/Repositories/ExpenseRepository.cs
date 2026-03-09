using Microsoft.EntityFrameworkCore;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Services
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            CreateAsync(Expense expense, List<ExpenseShare> shares, int currentUserId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == expense.GroupId);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {expense.GroupId} does not exist."));
                return (null, errors);
            }

            var groupMembers = await _context.GroupMembers.AsNoTracking()
                .Where(m => m.GroupId == expense.GroupId && m.IsDeleted == false)
                .ToListAsync();

            if (!groupMembers.Any(m => m.UserId == currentUserId))
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var validMemberIds = groupMembers.Select(m => m.Id).ToHashSet();
            if (!validMemberIds.Contains(expense.PaidByGroupMemberId))
            {
                errors.Add(("ValidationError", $"PaidByGroupMemberId {expense.PaidByGroupMemberId} does not belong to this group."));
                return (null, errors);
            }

            var invalidShareMembers = shares
                .Where(s => !validMemberIds.Contains(s.GroupMemberId))
                .Select(s => s.GroupMemberId)
                .ToList();

            if (invalidShareMembers.Any())
            {
                errors.Add(("ValidationError", $"The following GroupMemberIds do not belong to this group: {string.Join(", ", invalidShareMembers)}."));
                return (null, errors);
            }

            foreach (var share in shares)
                share.CreatedBy = currentUserId;

            foreach (var share in shares)
                expense.Shares.Add(share);

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();


            return (expense, null);
        }

        public async Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            UpdateAsync(int groupId, int expenseId, Expense updatedExpense, List<ExpenseShare> newShares, int currentUserId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {groupId} does not exist."));
                return (null, errors);
            }

            var existing = await _context.Expenses
                .Include(e => e.Shares)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.IsDeleted == false);

            if (existing == null)
            {
                errors.Add(("NotFound", $"Expense with id {expenseId} does not exist."));
                return (null, errors);
            }

            if (existing.GroupId != groupId)
            {
                errors.Add(("NotFound", $"Expense {expenseId} does not belong to group {groupId}."));
                return (null, errors);
            }

            var groupMembers = await _context.GroupMembers.AsNoTracking()
                .Where(m => m.GroupId == groupId && m.IsDeleted == false)
                .ToListAsync();

            if (!groupMembers.Any(m => m.UserId == currentUserId))
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var validMemberIds = groupMembers.Select(m => m.Id).ToHashSet();
            if (!validMemberIds.Contains(updatedExpense.PaidByGroupMemberId))
            {
                errors.Add(("ValidationError", $"PaidByGroupMemberId {updatedExpense.PaidByGroupMemberId} does not belong to this group."));
                return (null, errors);
            }

            var invalidShareMembers = newShares
                .Where(s => !validMemberIds.Contains(s.GroupMemberId))
                .Select(s => s.GroupMemberId)
                .ToList();

            if (invalidShareMembers.Any())
            {
                errors.Add(("ValidationError", $"The following GroupMemberIds do not belong to this group: {string.Join(", ", invalidShareMembers)}."));
                return (null, errors);
            }

            existing.Description = updatedExpense.Description;
            existing.TotalAmount = updatedExpense.TotalAmount;
            existing.PaidByGroupMemberId = updatedExpense.PaidByGroupMemberId;
            existing.ExpenseDate = updatedExpense.ExpenseDate;
            existing.LastModifiedBy = currentUserId;

            _context.ExpenseShares.RemoveRange(existing.Shares);

            foreach (var share in newShares)
            {
                share.CreatedBy = currentUserId;
                existing.Shares.Add(share);
            }

            await _context.SaveChangesAsync();

            return (existing, null);
        }

        public async Task<(Expense? Expense, List<(string Type, string Message)>? Errors)>
            GetByIdAsync(int groupId, int expenseId, int currentUserId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {groupId} does not exist."));
                return (null, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId && m.IsDeleted == false);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var expense = await _context.Expenses.AsNoTracking()
                .Include(e => e.Shares)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.GroupId == groupId && e.IsDeleted == false);

            if (expense == null)
            {
                errors.Add(("NotFound", $"Expense with id {expenseId} does not exist in group {groupId}."));
                return (null, errors);
            }

            return (expense, null);
        }

        public async Task<(List<Expense>? Expenses, List<(string Type, string Message)>? Errors)>
            GetAllByGroupAsync(int groupId, int currentUserId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {groupId} does not exist."));
                return (null, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId && m.IsDeleted == false);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (null, errors);
            }

            var expenses = await _context.Expenses.AsNoTracking()
                .Include(e => e.Shares)
                .Where(e => e.GroupId == groupId && e.IsDeleted == false)
                .ToListAsync();

            return (expenses, null);
        }

        public async Task<(bool Success, List<(string Type, string Message)>? Errors)>
            SoftDeleteAsync(int groupId, int expenseId, int currentUserId)
        {
            var errors = new List<(string Type, string Message)>();

            var groupExists = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
            {
                errors.Add(("NotFound", $"Group with id {groupId} does not exist."));
                return (false, errors);
            }

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.GroupId == groupId && e.IsDeleted == false);

            if (expense == null)
            {
                errors.Add(("NotFound", $"Expense with id {expenseId} does not exist in group {groupId}."));
                return (false, errors);
            }

            var isMember = await _context.GroupMembers.AsNoTracking()
                .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId && m.IsDeleted == false);

            if (!isMember)
            {
                errors.Add(("Forbidden", "You are not a member of this group."));
                return (false, errors);
            }

            expense.IsDeleted = true;
            expense.IsActive = false;
            expense.LastModifiedAt = DateTime.UtcNow;
            expense.LastModifiedBy = currentUserId;

            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
