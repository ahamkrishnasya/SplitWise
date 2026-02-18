using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupRequestDto request)
        {
            var result = await _groupService.CreateGroupAsync(request);
            if (result == null)
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Failed to create group.",
                        errorType: "CreationError",
                        errorMessage: "Group creation failed",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }
            else
            {
                return CreatedAtAction(
                    nameof(GetGroupById),
                    new { id = result.GroupId },
                    ApiResponseFactory.Success(
                        data: result,
                        message: "Group created successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status201Created
                    )
                );
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);

            if (group == null)
            {
                return NotFound(
                    ApiResponseFactory.Failure<object>(
                        message: "Group not found.",
                        errorType: "NotFound",
                        errorMessage: $"No group found with Id:{id}",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }
            return Ok(
                ApiResponseFactory.Success<object>(
                    data: group,
                    message: "Group data retrieved successfully.",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers([FromBody] GroupMemberRequestDto request)
        {
            var member = _groupService.AddMembers(request);
        }
    }
}
