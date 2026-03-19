using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Friendships;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SplitWise.Application.DTOs.EmailVerification;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/")]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost]
        [Route("friendships")]
        public async Task<IActionResult> AddFriends(FriendshipRequestDto request)
        {
            var result = await _friendshipService.AddFriends(request);
            if (result == null)
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Failed to add Friend.",
                        errorType: "CreationError",
                        errorMessage: "Friend addition failed",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }
            else
            {
                return StatusCode(statusCode: StatusCodes.Status201Created,
                    ApiResponseFactory.Success(
                        data: result,
                        message: "Friend added successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status201Created
                    )
                );
            }
        }

        [HttpGet]
        [Route("friendships")]
        public async Task<IActionResult> GetFriends()
        {
            var friends = await _friendshipService.GetFriends();

            return Ok(
                ApiResponseFactory.Success(
                    data: friends,
                    message: "Friends retrieved successfully.",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );  
        }

        [HttpGet]
        [Route("friendships-nonFriends")]
        public async Task<IActionResult> NotInFriends()
        {
            var result = await _friendshipService.NotInFriends();

            return Ok(
                ApiResponseFactory.Success(
                    data: result,
                    message: "These Users are not friends yet",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        [HttpPost]
        [Route("friendships-invitations")]
        public async Task<IActionResult> InviteFriend(EmailInvitationRequestDto request)
        {
            var result = await _friendshipService.InviteFriend(request);
            if ( result == null )
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Failed To Invite",
                        errorType: "InvitationError",
                        errorMessage: "Invitation_Failed",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            string message = result.Status switch
            {
                "Invited" => "Invitation sent successfully.",
                "Already_Exist" => "Invitation already sent. Please wait before resending.",
                "Resend" => "Invitation resent successfully.",
                "Already_Added" => "You are already friends.",
                "Friend_Added" => "Friend added successfully.",
                _ => "Invitation processed."
            };

            return Ok(
                ApiResponseFactory.Success<object>(
                    data: result,
                    message: message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );

        }

        [HttpGet]
        [Route("friendships-pendingInvites")]
        public async Task<IActionResult> PendingInvites()
        {
            var result = await _friendshipService.PendingInvites();

            return Ok(
                ApiResponseFactory.Success<object>(
                    data: result,
                    message: "Pending Invites",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }
    }

    
}
