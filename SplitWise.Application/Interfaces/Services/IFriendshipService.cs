using SplitWise.Application.DTOs.Friendships;

namespace SplitWise.Application.Interfaces.Services
{
    public interface IFriendshipService
    {
        Task<FriendshipResponseDto> CreateFriendshipAsync(int id);
        Task<List<FriendshipResponseDto>> GetFriends();
    }
}
