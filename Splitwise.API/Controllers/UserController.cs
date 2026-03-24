using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var (data, errors) = await _userService.GetProfileAsync();

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Profile retrieved successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            return NotFound(
                ApiResponseFactory.Failure<object>(
                    message: "User not found.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status404NotFound
                )
            );
        }

        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] PatchUserRequestDto request)
        {
            var (data, errors) = await _userService.UpdateProfileAsync(request);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Profile updated successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "NotFound")
            {
                return NotFound(
                    ApiResponseFactory.Failure<object>(
                        message: "User not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Profile update failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var (success, errors) = await _userService.ChangePasswordAsync(request);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "Password changed successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "NotFound")
            {
                return NotFound(
                    ApiResponseFactory.Failure<object>(
                        message: "User not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Password change failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequestDto request)
        {
            var (success, errors) = await _userService.DeleteAccountAsync(request);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "Account has been permanently deactivated.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "NotFound")
            {
                return NotFound(
                    ApiResponseFactory.Failure<object>(
                        message: "User not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            if (firstError.Type == "InvalidCredentials")
            {
                return Unauthorized(
                    ApiResponseFactory.Failure<object>(
                        message: "Account deletion failed.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status401Unauthorized
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Account deletion failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}
