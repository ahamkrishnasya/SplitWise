using System.ComponentModel.DataAnnotations;
using SplitWise.Domain.Common;

namespace SplitWise.Domain.Entities
{
    public class User : BaseEntity
    {
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        [MinLength(2)]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z ']+$")]
        public string? FirstName { get; set; }

        [MinLength(2)]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z ']+$")]
        public string? LastName { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new HashSet<GroupMember>();
        public virtual ICollection<Friendship> FriendshipsInitiated { get; set; } = new HashSet<Friendship>();
        public virtual ICollection<Friendship> FriendshipsReceived { get; set; } = new HashSet<Friendship>();
        public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new HashSet<EmailVerificationToken>();
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new HashSet<PasswordResetToken>();
    }
}
