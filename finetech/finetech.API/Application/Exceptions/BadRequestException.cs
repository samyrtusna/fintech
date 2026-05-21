namespace fintech.API.Application.Exceptions
{
    public class BadRequestException(string message) : ApiException(StatusCodes.Status400BadRequest, message)
    {
    }
}
