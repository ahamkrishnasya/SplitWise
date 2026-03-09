
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Expenses;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups/{groupId:int}/expenses")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense(int groupId, [FromBody] CreateExpenseRequestDto request)
        {
            var (data, errors) = await _expenseService.CreateAsync(groupId, request);

            if (data != null)
            {
                return StatusCode(StatusCodes.Status201Created,
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Expense created successfully.",
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
                    message: "Expense creation failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpPut]
        [Route("{expenseId:int}")]
        public async Task<IActionResult> UpdateExpense(int groupId, int expenseId, [FromBody] CreateExpenseRequestDto request)
        {
            var (data, errors) = await _expenseService.UpdateAsync(groupId, expenseId, request);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Expense updated successfully.",
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
                    message: "Expense update failed.",
                    errorType: firstError.Type,
                    errorMessage: firstError.Message,
                    httpContext: HttpContext,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses(int groupId)
        {
            var (data, errors) = await _expenseService.GetAllByGroupAsync(groupId);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Expenses retrieved successfully.",
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

        [HttpGet]
        [Route("{expenseId:int}")]
        public async Task<IActionResult> GetExpenseById(int groupId, int expenseId)
        {
            var (data, errors) = await _expenseService.GetByIdAsync(groupId, expenseId);

            if (data != null)
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: data,
                        message: "Expense retrieved successfully.",
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

        [HttpDelete]
        [Route("{expenseId:int}")]
        public async Task<IActionResult> DeleteExpense(int groupId, int expenseId)
        {
            var (success, errors) = await _expenseService.SoftDeleteAsync(groupId, expenseId);

            if (success)
            {
                return Ok(
                    ApiResponseFactory.Success<object?>(
                        data: null,
                        message: "Expense deleted successfully.",
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
