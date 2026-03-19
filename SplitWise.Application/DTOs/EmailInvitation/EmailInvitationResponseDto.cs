using SplitWise.Application.DTOs.Friendships;

namespace SplitWise.Application.DTOs.EmailVerification
{
    public class EmailInvitationResponseDto : FriendshipResponseDto
    {
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
