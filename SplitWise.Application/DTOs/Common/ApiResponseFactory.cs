using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SplitWise.Application.DTOs.Common
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(
            T data,
            string message,
            HttpContext httpContext,
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                statusCode = statusCode,
                success = true,
                message = message,
                data = data,
                errors = null,
                meta = CreateMetaData(httpContext)
            };
        }

        public static ApiResponse<T> Failure<T>(
            string message,
            string errorType,
            string errorMessage,
            HttpContext httpContext,
            int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                statusCode = statusCode,
                success = false,
                message = message,
                data = default(T),
                errors = new List<ApiError>
                {
                    new ApiError
                    {
                        type = errorType,
                        message = errorMessage,
                    }
                },
                meta = CreateMetaData(httpContext)
            };
        }
        private static MetaData CreateMetaData(HttpContext httpContext)
        {
            return new MetaData
            {
                requestId = httpContext.TraceIdentifier,
                timeStamp = DateTime.UtcNow.ToString("o")
            };
        }
    }
}
