using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.PasswordResets;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Route("api/password-resets")]
    public class PasswordResetsController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;

        public PasswordResetsController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto request)
        {
            var (_, errors) = await _passwordResetService.RequestPasswordResetAsync(request.Email ?? string.Empty);

            if (errors != null && errors.Any())
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Password reset request failed",
                        errorType: errors[0].Type,
                        errorMessage: errors[0].Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            return Ok(
                ApiResponseFactory.Success<object>(
                    data: null,
                    message: "If an account with this email exists and is verified, a password reset link has been sent.",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var (_, errors) = await _passwordResetService.ResetPasswordAsync(
                request.Token ?? string.Empty,
                request.NewPassword ?? string.Empty);

            if (errors != null && errors.Any())
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Password reset failed",
                        errorType: errors[0].Type,
                        errorMessage: errors[0].Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            return Ok(
                ApiResponseFactory.Success<object>(
                    data: null,
                    message: "Password has been reset successfully.",
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status200OK
                )
            );
        }
    }
}
