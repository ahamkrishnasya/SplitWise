namespace SplitWise.Domain.Common.Exceptions
{
    public class ConflictExcetion : AppException
    {
        public ConflictExcetion(string message) : base(message, "Conflict") { }
    }
}
