using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/activities")]
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            var result = await _activityService.GetActivities();

            return Ok(
                ApiResponseFactory.Success<object>(
                    data: result,
                    message: "Activities",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }
    }
}
