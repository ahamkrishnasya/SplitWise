using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;  
                context.Response.ContentType = "application/json";

                var response = ApiResponseFactory.Failure<object>(
                    "An unexpected error occured",
                    "SERVER_ERROR",
                    ex.Message,
                    context,
                    StatusCodes.Status500InternalServerError
                );

                await context.Response.WriteAsJsonAsync( response );
            }
        }
    }
}
