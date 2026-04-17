namespace fintech.Application.Exceptions
{
    public class DuplicateValueException : ApiException
    {
        public DuplicateValueException(string message) : base(StatusCodes.Status409Conflict, message)
        { }
    }
}
