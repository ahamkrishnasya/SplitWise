namespace SplitWise.Domain.Common.Exceptions;

public abstract class AppException : Exception
{
    public string ErrorType { get; } 
    public string? ErrorCode { get; }
    public object? Details { get; }

    protected AppException(
        string message,
        string errorType,
        string? errorcode = null,
        object? details = null)
        : base(message)
    {
        ErrorType = errorType;
        ErrorCode = errorcode;
        Details = details;
    }
}
