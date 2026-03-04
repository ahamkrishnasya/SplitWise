using SplitWise.Domain.Common;

namespace SplitWise.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
