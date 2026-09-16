using fintech.API.API.Filters;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;

namespace fintech.Tests.API.Filters
{
    public class ValidationFilterTests
    {
        // ============================================================
        // Test DTO
        // ============================================================

        public class TestRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        // ============================================================
        // Test Validator
        // ============================================================

        private class TestRequestValidator : AbstractValidator<TestRequest>
        {
            public TestRequestValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Name is required.");

                RuleFor(x => x.Name)
                    .MinimumLength(3)
                    .WithMessage("Name must be at least 3 characters long.");
            }
        }

        // ============================================================
        // Helpers
        // ============================================================

        private static ActionExecutingContext CreateContext(
            Dictionary<string, object?> arguments,
            HttpContext httpContext)
        {
            var actionContext = new ActionContext(
                httpContext,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                arguments,
                controller: null);
        }

        private static DefaultHttpContext CreateHttpContext(
          IValidator<TestRequest>? validator = null)
        {
            var context = new DefaultHttpContext();

            var services = new ServiceCollection();

            if (validator != null)
            {
                services.AddSingleton<IValidator<TestRequest>>(validator);
            }

            context.RequestServices = services.BuildServiceProvider();

            return context;
        }

        private static async Task<Dictionary<string, object>> ReadResponseBody(
            HttpContext context)
        {
            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body);
            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<Dictionary<string, object>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        private static async Task<IActionResult> ExecuteFilterAsync(
            ActionExecutingContext context,
            IValidator<TestRequest>? validator,
            ActionExecutionDelegate next)
        {
            context.HttpContext.RequestServices =
                CreateHttpContext(validator).RequestServices;

            var filter = new ValidationFilter();

            await filter.OnActionExecutionAsync(context, next);

            return context.Result!;
        }

        // ============================================================
        // Tests
        // ============================================================

        [Fact]
        public async Task OnActionExecutionAsync_NullArgument_CallsNext()
        {
            // Arrange
            var httpContext = CreateHttpContext();

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = null
                },
                httpContext);

            var nextCalled = false;

            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;

                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
            Assert.Null(context.Result);
        }

        [Fact]
        public async Task OnActionExecutionAsync_NoArguments_CallsNext()
        {
            // Arrange
            var httpContext = CreateHttpContext();

            var context = CreateContext(
                new Dictionary<string, object?>(),
                httpContext);

            var nextCalled = false;

            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;

                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
            Assert.Null(context.Result);
        }

        [Fact]
        public async Task OnActionExecutionAsync_ValidArgument_CallsNext()
        {
            // Arrange
            var validator = new TestRequestValidator();
            var httpContext = CreateHttpContext(validator);

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = new TestRequest
                    {
                        Name = "John"
                    }
                },
                httpContext);

            var nextCalled = false;

            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;

                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
            Assert.Null(context.Result);
        }

        [Fact]
        public async Task OnActionExecutionAsync_InvalidArgument_ReturnsBadRequest()
        {
            // Arrange
            var validator = new TestRequestValidator();
            var httpContext = CreateHttpContext(validator);

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = new TestRequest
                    {
                        Name = ""
                    }
                },
                httpContext);

            var next = new Mock<ActionExecutionDelegate>();

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(
                context,
                next.Object);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);

            Assert.Equal(
                StatusCodes.Status400BadRequest,
                result.StatusCode);

            next.Verify(
                x => x(),
                Times.Never);
        }

        [Fact]
        public async Task OnActionExecutionAsync_InvalidArgument_ReturnsValidationFailedMessage()
        {
            // Arrange
            var validator = new TestRequestValidator();
            var httpContext = CreateHttpContext(validator);

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = new TestRequest
                    {
                        Name = ""
                    }
                },
                httpContext);

            ActionExecutionDelegate next = () =>
            {
                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);

            Assert.NotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);

            Assert.Contains(
                "Validation failed",
                json);
        }

        [Fact]
        public async Task OnActionExecutionAsync_InvalidArgument_GroupsErrorsByPropertyName()
        {
            // Arrange
            var validator = new TestRequestValidator();
            var httpContext = CreateHttpContext(validator);

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = new TestRequest
                    {
                        Name = ""
                    }
                },
                httpContext);

            ActionExecutionDelegate next = () =>
            {
                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);

            var json = JsonSerializer.Serialize(result.Value);

            using var document = JsonDocument.Parse(json);

            var errors = document.RootElement
                .GetProperty("Errors");

            var nameErrors = errors
                .GetProperty("Name");

            Assert.Equal(
                JsonValueKind.Array,
                nameErrors.ValueKind);

            Assert.Equal(
                2,
                nameErrors.GetArrayLength());

            Assert.Contains(
                "Name is required.",
                nameErrors.EnumerateArray()
                    .Select(x => x.GetString()));

            Assert.Contains(
                "Name must be at least 3 characters long.",
                nameErrors.EnumerateArray()
                    .Select(x => x.GetString()));
        }

        [Fact]
        public async Task OnActionExecutionAsync_MultipleInvalidArguments_AggregatesErrors()
        {
            // Arrange
            var firstValidator = new TestRequestValidator();
            var secondValidator = new TestRequestValidator();

            var services = new ServiceCollection();

            services.AddSingleton<IValidator<TestRequest>>(firstValidator);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["first"] = new TestRequest
                    {
                        Name = ""
                    },
                    ["second"] = new TestRequest
                    {
                        Name = ""
                    }
                },
                httpContext);

            ActionExecutionDelegate next = () =>
            {
                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);

            var json = JsonSerializer.Serialize(result.Value);

            using var document = JsonDocument.Parse(json);

            var nameErrors = document.RootElement
                .GetProperty("Errors")
                .GetProperty("Name");

            Assert.Equal(
                4,
                nameErrors.GetArrayLength());
        }

        [Fact]
        public async Task OnActionExecutionAsync_PassesRequestAbortedTokenToValidator()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();

            var expectedToken = cancellationTokenSource.Token;

            var validatorMock =
                new Mock<IValidator<TestRequest>>();

            validatorMock
                .Setup(x => x.ValidateAsync(
                    It.IsAny<IValidationContext>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult())
                .Callback<IValidationContext, CancellationToken>(
                    (_, token) =>
                    {
                        Assert.Equal(expectedToken, token);
                    });

            var httpContext = CreateHttpContext(
                validatorMock.Object);

            httpContext.RequestAborted = expectedToken;

            var context = CreateContext(
                new Dictionary<string, object?>
                {
                    ["request"] = new TestRequest
                    {
                        Name = "John"
                    }
                },
                httpContext);

            ActionExecutionDelegate next = () =>
            {
                var executedContext = new ActionExecutedContext(
                    context,
                    new List<IFilterMetadata>(),
                    null);

                return Task.FromResult(executedContext);
            };

            var filter = new ValidationFilter();

            // Act
            await filter.OnActionExecutionAsync(context, next);

            // Assert
            validatorMock.Verify(
                x => x.ValidateAsync(
                    It.IsAny<IValidationContext>(),
                    expectedToken),
                Times.Once);
        }
    }
}