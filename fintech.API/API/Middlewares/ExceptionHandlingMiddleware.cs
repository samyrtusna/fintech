using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.Exceptions;

namespace fintech.API.API.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogWarning(ex, "Argument null encountered: {Message}", ex.Message);
                await WriteResponse(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ApiException ex)
            {
                logger.LogWarning(ex, "API error (status {StatusCode}): {Message}", ex.StatusCode, ex.Message);
                await WriteResponse(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await WriteResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponsesDto<string>(statusCode, message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
