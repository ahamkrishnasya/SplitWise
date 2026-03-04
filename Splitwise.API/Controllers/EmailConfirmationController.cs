using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Route("api/email-confirmations")]
    public class EmailConfirmationController : ControllerBase
    {
        private readonly IEmailVerificationService _emailVerificationService;

        public EmailConfirmationController(IEmailVerificationService emailVerificationService)
        {
            _emailVerificationService = emailVerificationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(
                    ApiResponseFactory.Failure<object>(
                        message: "Email verification failed",
                        errorType: "INVALID_TOKEN",
                        errorMessage: "Token is required",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            var (data, errors) = await _emailVerificationService.VerifyEmailAsync(token);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Email verified successfully. You may now log in.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var errorType = errors![0].Type;
            var errorMessage = errors![0].Message;

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Email verification failed",
                    errorType: errorType,
                    errorMessage: errorMessage,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}
