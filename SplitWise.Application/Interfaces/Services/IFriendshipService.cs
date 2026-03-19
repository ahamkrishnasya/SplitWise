using SplitWise.Application.DTOs.EmailVerification;
using SplitWise.Application.DTOs.Friendships;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IFriendshipService
    {
        Task<List<FriendshipResponseDto>> AddFriends(FriendshipRequestDto request);
        Task<List<FriendshipResponseDto>> GetFriends();
        Task<List<FriendshipResponseDto>> NotInFriends();
        Task<EmailInvitationResponseDto> InviteFriend(EmailInvitationRequestDto request);
        Task<List<FriendshipResponseDto>> PendingInvites();
    }
}
