using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Friendships;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/friendships")]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost]
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
    }

    
}
