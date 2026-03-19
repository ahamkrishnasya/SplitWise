using Azure.Core;
using SplitWise.Application.DTOs.EmailVerification;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public EmailVerificationService(
            IEmailVerificationRepository emailVerificationRepository,
            IFriendshipRepository friendshipRepository
            )
        {
            _emailVerificationRepository = emailVerificationRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task<(VerifyEmailResponseDto? Data, List<(string Type, string Message)>? Errors)> VerifyEmailAsync(string token)
        {
            var emailToken = await _emailVerificationRepository.GetByTokenAsync(token);

            if (emailToken == null)
            {
                return (null, new List<(string, string)>
                {
                    ("INVALID_TOKEN", "Verification token is invalid or does not exist")
                });
            }

            if (emailToken.ExpiresAt < DateTime.UtcNow)
            {
                return (null, new List<(string, string)>
                {
                    ("TOKEN_EXPIRED", "Verification token has expired")
                });
            }

            if (emailToken.IsUsed)
            {
                return (new VerifyEmailResponseDto { EmailVerified = true }, null);
            }

            emailToken.IsUsed = true;
            emailToken.User.EmailConfirmed = true;

            await _emailVerificationRepository.SaveChangesAsync();

            var emailInvitation = await _friendshipRepository.GetInvitation(emailToken.User.Email);
            bool hasValidInvite = emailInvitation != null && !emailInvitation.IsUsed && emailInvitation.ExpiresAt >= DateTime.UtcNow;

            if (hasValidInvite)
            {
                Friendship friend = new Friendship();
                if (emailToken.User.Id != emailInvitation.CreatedBy)
                {
                    friend.UserId1 = Math.Min(emailInvitation.CreatedBy, emailToken.User.Id);
                    friend.UserId2 = Math.Max(emailInvitation.CreatedBy, emailToken.User.Id);

                    var exist = await _friendshipRepository.IsExist(friend);

                    if (exist == null)
                    {
                        friend.CreatedBy = emailInvitation.CreatedBy;
                        await _friendshipRepository.AddAsync([friend]);
                        await _friendshipRepository.UpdateInvitation(emailInvitation.Id);
                    }
                }
            }

            return (new VerifyEmailResponseDto { EmailVerified = true }, null);
        }
    }
}
