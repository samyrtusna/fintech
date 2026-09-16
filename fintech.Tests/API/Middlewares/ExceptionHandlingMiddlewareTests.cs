using fintech.API.API.Middlewares;
using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace fintech.Tests.API.Middlewares
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _logger;

        public ExceptionHandlingMiddlewareTests()
        {
            _logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        }

        private ExceptionHandlingMiddleware CreateMiddleware(
            RequestDelegate next)
        {
            return new ExceptionHandlingMiddleware(
                next,
                _logger.Object);
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();

            context.Response.Body = new MemoryStream();

            return context;
        }

        private static async Task<string> ReadResponseBody(
            HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(context.Response.Body);

            return await reader.ReadToEndAsync();
        }

        // =========================================================
        // Successful request
        // =========================================================

        [Fact]
        public async Task Invoke_NoException_CallsNextAndDoesNotModifyResponse()
        {
            // Arrange
            var nextCalled = false;

            RequestDelegate next = context =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(200, context.Response.StatusCode);
        }

        // =========================================================
        // ArgumentNullException
        // =========================================================

        [Fact]
        public async Task Invoke_ArgumentNullException_Returns400()
        {
            // Arrange
            RequestDelegate next = _ =>
                throw new ArgumentNullException(
                    "user",
                    "User cannot be null.");

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(
                StatusCodes.Status400BadRequest,
                context.Response.StatusCode);

            Assert.StartsWith(
                "application/json",
                context.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_ArgumentNullException_ReturnsExceptionMessage()
        {
            // Arrange
            var exception = new ArgumentNullException(
                "user",
                "User cannot be null.");

            RequestDelegate next = _ => throw exception;

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            var responseBody = await ReadResponseBody(context);

            // Assert
            var response =
                JsonSerializer.Deserialize<ApiResponsesDto<string>>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            Assert.NotNull(response);
            Assert.Equal(
                StatusCodes.Status400BadRequest,
                response.StatusCode);

            Assert.Contains(
                exception.Message,
                response.Errors);
        }

        // =========================================================
        // ApiException
        // =========================================================

        [Fact]
        public async Task Invoke_ApiException_ReturnsExceptionStatusCode()
        {
            // Arrange
            var exception = new BadRequestException(
                "Invalid request.");

            RequestDelegate next = _ => throw exception;

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(
                exception.StatusCode,
                context.Response.StatusCode);

            Assert.StartsWith(
                "application/json",
                context.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_ApiException_ReturnsExceptionMessage()
        {
            // Arrange
            var exception = new BadRequestException(
                "Invalid category.");

            RequestDelegate next = _ => throw exception;

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            var responseBody = await ReadResponseBody(context);

            // Assert
            var response =
                JsonSerializer.Deserialize<ApiResponsesDto<string>>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            Assert.NotNull(response);
            Assert.Equal(
                exception.StatusCode,
                response.StatusCode);
            Assert.Contains(
                "Invalid category.",
                response.Errors); ;
        }

        // =========================================================
        // Unexpected Exception
        // =========================================================

        [Fact]
        public async Task Invoke_UnexpectedException_Returns500()
        {
            // Arrange
            RequestDelegate next = _ =>
                throw new InvalidOperationException(
                    "Something went wrong.");

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                context.Response.StatusCode);

            Assert.StartsWith(
                "application/json",
                context.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_UnexpectedException_DoesNotExposeOriginalExceptionMessage()
        {
            // Arrange
            RequestDelegate next = _ =>
                throw new InvalidOperationException(
                    "SECRET INTERNAL ERROR");

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            var responseBody = await ReadResponseBody(context);

            // Assert
            Assert.DoesNotContain(
                "SECRET INTERNAL ERROR",
                responseBody);

            Assert.Contains(
                "An unexpected error occurred.",
                responseBody);
        }

        // =========================================================
        // Logging
        // =========================================================

        [Fact]
        public async Task Invoke_UnexpectedException_LogsError()
        {
            // Arrange
            RequestDelegate next = _ =>
                throw new InvalidOperationException(
                    "Unexpected error.");

            var middleware = CreateMiddleware(next);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}