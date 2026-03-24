using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.EmailChange;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    public class EmailChangeController : ControllerBase
    {
        private readonly IEmailChangeService _emailChangeService;

        public EmailChangeController(IEmailChangeService emailChangeService)
        {
            _emailChangeService = emailChangeService;
        }

        // POST /api/email-changes
        [Authorize]
        [HttpPost("api/email-changes")]
        public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeDto request)
        {
            var (success, errors) = await _emailChangeService.RequestEmailChangeAsync(request);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "A verification link has been sent to your new email address.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "EmailAlreadyExists")
            {
                return Conflict(
                    ApiResponseFactory.Failure<object>(
                        message: "Email address is already in use.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status409Conflict
                    )
                );
            }

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
                    message: "Email change request failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        // PUT /api/email-changes
        [HttpPut("api/email-changes")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto request)
        {
            var (success, errors) = await _emailChangeService.ConfirmEmailChangeAsync(request);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "Email address updated successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "EmailAlreadyExists")
            {
                return Conflict(
                    ApiResponseFactory.Failure<object>(
                        message: "Email address is no longer available.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status409Conflict
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Email change confirmation failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}
