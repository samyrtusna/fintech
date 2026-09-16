using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fintech.API.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var cancellationToken =
                context.HttpContext.RequestAborted;

            var arguments = context.ActionArguments;

            var failures = new List<ValidationFailure>();

            foreach (var argument in arguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType =
                    typeof(IValidator<>).MakeGenericType(argument.GetType());

                var validator =
                    context.HttpContext.RequestServices
                        .GetRequiredService(validatorType);

                var validationContext =
                    new ValidationContext<object>(argument);

                if (validator is not IValidator asyncValidator)
                    continue;

                var result =
                    await asyncValidator.ValidateAsync(
                        validationContext,
                        cancellationToken);

                if (!result.IsValid)
                {
                    failures.AddRange(result.Errors);
                }
            }

            if (failures.Count != 0)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = failures
                        .GroupBy(f => f.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                });

                return;
            }

            await next();
        }
    }
}