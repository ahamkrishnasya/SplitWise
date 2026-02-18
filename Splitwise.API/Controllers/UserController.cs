using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
        {
            var (data, errors) = await _userService.RegisterAsync(request);

            if (data != null)
            {
                return StatusCode(StatusCodes.Status201Created,
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Registration successful.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status201Created
                    )
                );
            }

            if (errors.Any(e => e.Type == "EMAIL_ALREADY_EXISTS"))
            {
                return Conflict(
                    ApiResponseFactory.Failure<object>(
                        message: "User with the provided email already exists",
                        errorType: "EMAIL_ALREADY_EXISTS",
                        errorMessage: "An account with this email already exists",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status409Conflict
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "User registration failed",
                    errorType: "VALIDATION_ERROR",
                    errorMessage: "One or more validation errors occurred",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}
