using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.EmailVerification;
using SplitWise.Application.DTOs.Friendships;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _FriendshipRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IConfiguration _configuration;

        public FriendshipService(IFriendshipRepository FriendshipRepository, 
            ILoginRepository loginRepository, 
            IUserRepository userRepository, 
            IEmailService emailService, 
            IConfiguration configuration)
        {
            _FriendshipRepository = FriendshipRepository;
            _loginRepository = loginRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
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

                    var exist = await _FriendshipRepository.IsExist(friend);

                    if (exist != null)
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

            return await _FriendshipRepository.GetAsync(userid);
        }

        public async Task<List<FriendshipResponseDto>> NotInFriends()
        {
            var userId = _loginRepository.GetUserId();

            var notInFriends = await _FriendshipRepository.NotInFriends(userId);

            List<FriendshipResponseDto> dto = new List<FriendshipResponseDto>();

            foreach(var user in notInFriends)
            {
                FriendshipResponseDto friendshipdto = new FriendshipResponseDto();
                friendshipdto.Id = user.Id;
                friendshipdto.FirstName = user.FirstName;
                friendshipdto.LastName = user.LastName;
                friendshipdto.Email = user.Email;

                dto.Add(friendshipdto);
            }
            return dto;
        }

        public async Task<EmailInvitationResponseDto> InviteFriend(EmailInvitationRequestDto request)
        {
            var email = request.Email.Trim().ToLower();
            var userId = _loginRepository.GetUserId();
            var userEmail = await _userRepository.GetByEmailAsync(email);

            if(userEmail == null)
            {
                var user = await _userRepository.GetUserById(userId);
                var inviteExists = await _FriendshipRepository.InviteExists(email, userId); 

                if (inviteExists == null)
                {
                    var invitation = new FriendInvitation
                    {
                        CreatedBy = userId,
                        RecipientEmail = email
                    };

                    await _FriendshipRepository.AddInvite(invitation);

                    var invitationUrl = $"{_configuration["Frontend:BaseUrl"]}/Signup";
                    await _emailService.SendEmailInvitationAsync(email, invitationUrl, user);

                    return new EmailInvitationResponseDto
                    {
                        Id = invitation.Id,
                        Email = invitation.RecipientEmail,
                        ExpiresAt = invitation.ExpiresAt,
                        Status = "Invited"
                    };
                }
                else
                {
                    if(inviteExists.ExpiresAt > DateTime.UtcNow)
                    {
                        return new EmailInvitationResponseDto
                        {
                            Id = inviteExists.Id,
                            Email = inviteExists.RecipientEmail,
                            ExpiresAt = inviteExists.ExpiresAt,
                            Status = "Already_Exist"
                        };
                    }
                    else
                    {
                        inviteExists.LastModifiedAt = DateTime.UtcNow;
                        inviteExists.LastModifiedBy = userId;
                        inviteExists.ExpiresAt = DateTime.UtcNow.AddHours(24);
                        await _FriendshipRepository.UpdateInvite(inviteExists);
                        var invitationUrl = $"{_configuration["Frontend:BaseUrl"]}/Signup";

                        await _emailService.SendEmailInvitationAsync(email, invitationUrl, user);

                        return new EmailInvitationResponseDto
                        {
                            Id = inviteExists.Id,
                            Email = inviteExists.RecipientEmail,
                            ExpiresAt = inviteExists.ExpiresAt,
                            Status = "Resend"
                        };
                    }
                }
            }

            Friendship friend = new Friendship();
            if (userId != userEmail.Id && userEmail.EmailConfirmed == true)
            {
                friend.UserId1 = Math.Min(userEmail.Id, userId);
                friend.UserId2 = Math.Max(userEmail.Id, userId);

                var exist = await _FriendshipRepository.IsExist(friend);

                if (exist != null)
                {
                    return new EmailInvitationResponseDto
                    {
                        Id = exist.Id,
                        FriendUserId = exist.UserId1 == userId ? exist.UserId2 : userId,
                        CreatedByUserId = userId,
                        Status = "Already_Added"
                    };
                }
                friend.CreatedBy = userId;
                await _FriendshipRepository.AddAsync([friend]);
            }
            return new EmailInvitationResponseDto
            {
                Id = friend.Id,
                FriendUserId = friend.UserId1 == userId ? friend.UserId2 : userId,
                CreatedByUserId = userId,
                Status = "Friend_Added"
            };
        }

        public async Task<List<FriendshipResponseDto>> PendingInvites()
        {
            var userId = _loginRepository.GetUserId();
            var pendingInvites = await _FriendshipRepository.PendingInvites(userId);

            return pendingInvites.Select(x => new FriendshipResponseDto
            {
                Id = x.Id,
                Email = x.RecipientEmail,
                CreatedByUserId = x.CreatedBy
            }).ToList();
        }
    }
}
