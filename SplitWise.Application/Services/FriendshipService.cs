using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp;
using SplitWise.Application.DTOs.Friendships;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<FriendshipResponseDto>> AddFriends(FriendshipRequestDto request)
        {
            int userid = _loginRepository.GetUserId();

            List<Friendship> list = new List<Friendship>();

            foreach(var id in request.FriendUserId)
            {
                Friendship friend = new Friendship();
                if(userid != id)
                {
                    friend.UserId1 = Math.Min(id, userid);
                    friend.UserId2 = Math.Max(id, userid);

                    bool exists = await _FriendshipRepository.IsExist(friend);

                    if (exists)
                    {
                       continue;
                    }
                    friend.CreatedBy = userid;  
                    list.Add(friend);
                }
            }
            var result = await _FriendshipRepository.AddAsync(list);

            List<FriendshipResponseDto> response = new List<FriendshipResponseDto>();
            foreach(var item in result)
            {
                FriendshipResponseDto friendshipDto = new FriendshipResponseDto(); 
                friendshipDto.Id = item.Id;
                friendshipDto.CreatedByUserId = item.CreatedBy;
                friendshipDto.FriendUserId = (item.UserId1 ==  userid ? item.UserId2 : userid);

                response.Add(friendshipDto);    
            }

            return response;
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
