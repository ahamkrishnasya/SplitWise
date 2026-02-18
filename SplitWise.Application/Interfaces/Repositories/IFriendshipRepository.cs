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
        Task<bool> IsExist(int user1, int user2);
        Task<Friendship> AddAsync(Friendship group);
        Task<List<Friendship>> GetAsync(int userid);
    }
}
