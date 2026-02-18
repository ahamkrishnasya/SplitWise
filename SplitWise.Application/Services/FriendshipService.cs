using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.DTOs.Friendships;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp;

namespace SplitWise.Application.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _FriendshipRepository;
        private readonly ILoginRepository _loginRepository;

        public FriendshipService(IFriendshipRepository FriendshipRepository, ILoginRepository loginRepository)
        {
            _FriendshipRepository = FriendshipRepository;
            _loginRepository = loginRepository;
        }

        public async Task<FriendshipResponseDto> CreateFriendshipAsync(int friendId)
        {
            int userid = _loginRepository.GetUserId();

            if (friendId == userid) { 
                return null;
            }

            int user1 = Math.Min(friendId, userid);
            int user2 = Math.Max(friendId, userid);

            bool exists = await _FriendshipRepository.IsExist(user1, user2);

            if (exists)
            {
                return null;
            }

            var Friendship = new Friendship
            {
                UserId1 = user1,
                UserId2 = user2,
                CreatedBy = userid,
            };
            var result = await _FriendshipRepository.AddAsync(Friendship);

            return new FriendshipResponseDto
            {
                Id = result.Id,
                CreatedByUserId = result.UserId1,
                FriendUserId = result.UserId2,
            };
        }


        public async Task<List<FriendshipResponseDto>> GetFriends()
        {
            var userid = _loginRepository.GetUserId();

            var friends = await _FriendshipRepository.GetAsync(userid);

            List<FriendshipResponseDto> friendList = new List<FriendshipResponseDto>();
            foreach(var friend in friends)
            {
                FriendshipResponseDto dto = new FriendshipResponseDto();
                dto.Id = friend.Id;
                dto.FriendUserId = (friend.UserId1 == userid ? friend.UserId2 : friend.UserId1);
                dto.CreatedByUserId = friend.CreatedBy;
                friendList.Add(dto);
            }

            return friendList;
        }
    }
}
