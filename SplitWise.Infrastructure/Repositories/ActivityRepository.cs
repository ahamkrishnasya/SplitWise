using Microsoft.EntityFrameworkCore;
using SplitWise.Application.DTOs.Activities;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Infrastructure.Data;

namespace SplitWise.Infrastructure.Services
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;
        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityResponseDto>> GetActivities(int userId)
        {
            var activities = new List<ActivityResponseDto>();

            var userEmail = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            var users = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new ActivityResponseDto
                {
                    Type = "User",
                    Message =
                        u.EmailConfirmed
                            ? "Your email was verified"
                            : "Account created",
                    ActivityDate = u.LastModifiedAt ?? u.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(users);

            var passwordResets = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId)
                .Select(t => new ActivityResponseDto
                {
                    Type = "User",
                    Message = "Password reset requested",
                    ActivityDate = t.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(passwordResets);

            var groupMemberIds = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.Id)
                .ToListAsync();

            var friendships = await _context.Friendships
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Where(f => f.UserId1 == userId || f.UserId2 == userId)
                .Select(f => new ActivityResponseDto
                {
                    Type = "Friend",
                    Message = f.UserId1 == userId
                        ? $"You became friends with {f.User2.FirstName} {f.User2.LastName}"
                        : $"You became friends with {f.User1.FirstName} {f.User1.LastName}",
                    ActivityDate = f.LastModifiedAt ?? f.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(friendships);

            var groups = await _context.Groups
                .Where(g => g.GroupMembers.Any(m => m.UserId == userId))
                .Select(g => new ActivityResponseDto
                {
                    Type = "Group",
                    Message = $"Group created: {g.GroupName}",
                    ActivityDate = g.LastModifiedAt ?? g.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(groups);

            var groupMembers = await _context.GroupMembers
                .Include(gm => gm.User)
                .Include(gm => gm.Groups)
                .Where(gm =>
                    gm.UserId == userId ||                  // you added
                    gm.CreatedBy == userId ||               // you added someone
                    _context.GroupMembers                  // same group visibility
                        .Any(x => x.GroupId == gm.GroupId && x.UserId == userId)
                )
                .Select(gm => new ActivityResponseDto
                {
                    Type = "GroupMember",
                    Message =
                        gm.UserId == userId
                            ? $"You joined group '{gm.Groups.GroupName}'"
                            : gm.CreatedBy == userId
                                ? $"You added {gm.User.FirstName} to '{gm.Groups.GroupName}'"
                                : $"{gm.User.FirstName} was added to '{gm.Groups.GroupName}'",
                    ActivityDate = gm.LastModifiedAt ?? gm.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(groupMembers);

            var expenses = await _context.Expenses
                .Where(e =>
                    groupMemberIds.Contains(e.PaidByGroupMemberId) ||
                    e.Shares.Any(s => groupMemberIds.Contains(s.GroupMemberId))
                )
                .Select(e => new ActivityResponseDto
                {
                    Type = "Expense",
                    Message = $"Expense '{e.Description}' of ₹{e.TotalAmount}",
                    ActivityDate = e.LastModifiedAt ?? e.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(expenses);

            var shares = await _context.ExpenseShares
                .Where(s => groupMemberIds.Contains(s.GroupMemberId))
                .Select(s => new ActivityResponseDto
                {
                    Type = "Share",
                    Message = $"You owe/share ₹{s.Amount}",
                    ActivityDate = s.LastModifiedAt ?? s.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(shares);

            var settlements = await _context.Settlements
                .Include(s => s.ExpenseShare)
                .Where(s => groupMemberIds.Contains(s.ExpenseShare.GroupMemberId))
                .Select(s => new ActivityResponseDto
                {
                    Type = "Settlement",
                    Message = $"Settlement of ₹{s.Amount}",
                    ActivityDate = s.LastModifiedAt ?? s.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(settlements);

            var invitations = await _context.FriendInvitations
                .Include(i => i.CreatedByUser)
                .Where(i =>
                    i.CreatedByUser.Id == userId ||
                    i.RecipientEmail == userEmail
                )
                .Select(i => new ActivityResponseDto
                {
                    Type = "Invitation",
                    Message = $"Invitation sent to {i.RecipientEmail}",
                    ActivityDate = i.LastModifiedAt ?? i.CreatedAt
                })
                .ToListAsync();

            activities.AddRange(invitations);

            return activities
                .OrderByDescending(a => a.ActivityDate)
                .ToList();
        }
    }
}
