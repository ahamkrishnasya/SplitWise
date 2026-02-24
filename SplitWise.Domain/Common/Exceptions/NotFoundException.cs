namespace SplitWise.Domain.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, "NotFound") { }
    }
}
