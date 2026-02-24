using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        [Route("api/groups")]
        public async Task<IActionResult> Groups()
        {
            var result = await _groupService.Groups();

            return Ok(
                ApiResponseFactory.Success(
                    data: result,
                    message: "Groups found successfully.",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        [HttpPost]
        [Route("api/groups")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupRequestDto request)
        {
            var result = await _groupService.CreateGroupAsync(request);

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

        [HttpGet]
        [Route("api/groups/{id:int}")]
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
        [Route("api/groups/{id:int}/members")]
        public async Task<IActionResult> AddMembers([FromBody] GroupMemberRequestDto request, int id)
        {
            var result = await _groupService.AddMembers(request, id);

            if (result == null)
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Unable to add friend",
                        errorType: "CreationError",
                        errorMessage: "Member addition failed",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }
            else
            {
                return StatusCode(StatusCodes.Status201Created,
                    ApiResponseFactory.Success<object>(
                        data: result,
                        message: "Member added successfully",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status201Created
                    )
                );
            }
        }


    }
}
