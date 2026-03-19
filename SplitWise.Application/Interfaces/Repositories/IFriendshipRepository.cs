using SplitWise.Application.DTOs.Friendships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Friendship> IsExist(Friendship friend);
        Task<List<Friendship>> AddAsync(List<Friendship> friend);
        Task<List<FriendshipResponseDto>> GetAsync(int userid);
        Task<List<User>> NotInFriends(int userId);
        Task AddInvite(FriendInvitation invite);
        Task UpdateInvite(FriendInvitation invite);
        Task<FriendInvitation?> GetInvitation(string email);
        Task UpdateInvitation(int invitationId);
        Task<FriendInvitation> InviteExists(string email, int userId);
        Task<List<FriendInvitation>> PendingInvites(int userId);
    }
}
