namespace fintech.API.Application.Exceptions
{
    public class DuplicateValueException(string message) : ApiException(StatusCodes.Status409Conflict, message)
    {
    }
}
