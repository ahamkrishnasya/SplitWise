using SplitWise.Domain.Common;

namespace SplitWise.Domain.Entities
{
    public class FriendInvitation : BaseEntity
    {
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);
        public bool IsUsed { get; set; } = false;
        public string RecipientEmail { get; set; } = string.Empty;
        public User CreatedByUser { get; set; }
    }
}
