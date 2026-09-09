namespace fintech.API.Application.Exceptions
{
    public class UnauthorizedException(string message) : ApiException(StatusCodes.Status401Unauthorized, message)
    {
    }
}
