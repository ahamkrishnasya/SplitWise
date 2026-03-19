using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Settlements;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    public class SettlementController : ControllerBase
    {
        private readonly ISettlementService _settlementService;

        public SettlementController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        [HttpGet("api/groups/{groupId:int}/settlements")]
        public async Task<IActionResult> GetGroupSettlements(int groupId)
        {
            var (data, errors) = await _settlementService.GetGroupSettlementsAsync(groupId);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Settlements retrieved successfully.",
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
                        message: "Resource not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseFactory.Failure<object>(
                    message: "Access denied.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status403Forbidden
                )
            );
        }

        [HttpGet("api/expense-shares/{expenseShareId:int}/settlements")]
        public async Task<IActionResult> GetExpenseShareSettlements(int expenseShareId)
        {
            var (data, errors) = await _settlementService.GetExpenseShareSettlementsAsync(expenseShareId);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Settlements retrieved successfully.",
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
                        message: "Resource not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseFactory.Failure<object>(
                    message: "Access denied.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status403Forbidden
                )
            );
        }

        [HttpPost("api/expense-shares/{expenseShareId:int}/settlements")]
        public async Task<IActionResult> CreateSettlement(int expenseShareId, [FromBody] CreateSettlementRequestDto request)
        {
            var (data, errors) = await _settlementService.CreateSettlementAsync(expenseShareId, request);

            if (data != null)
            {
                return StatusCode(StatusCodes.Status201Created,
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Settlement created successfully.",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status201Created
                    )
                );
            }

            var firstError = errors!.First();

            if (firstError.Type == "NotFound")
            {
                return NotFound(
                    ApiResponseFactory.Failure<object>(
                        message: "Resource not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            if (firstError.Type == "Forbidden")
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseFactory.Failure<object>(
                        message: "Access denied.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status403Forbidden
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Settlement creation failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpPatch("api/settlements/{settlementId:int}")]
        public async Task<IActionResult> UpdateSettlement(int settlementId, [FromBody] PatchSettlementRequestDto request)
        {
            var (data, errors) = await _settlementService.UpdateSettlementAsync(settlementId, request);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Settlement updated successfully.",
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
                        message: "Resource not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            if (firstError.Type == "Forbidden")
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseFactory.Failure<object>(
                        message: "Access denied.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status403Forbidden
                    )
                );
            }

            return BadRequest(
                ApiResponseFactory.Failure<object>(
                    message: "Settlement update failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpDelete("api/settlements/{settlementId:int}")]
        public async Task<IActionResult> DeleteSettlement(int settlementId)
        {
            var (success, errors) = await _settlementService.DeleteSettlementAsync(settlementId);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "Settlement deleted successfully.",
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
                        message: "Resource not found.",
                        errorType: firstError.Type,
                        errorMessage: firstError.Message,
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status404NotFound
                    )
                );
            }

            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseFactory.Failure<object>(
                    message: "Access denied.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status403Forbidden
                )
            );
        }
    }
}
