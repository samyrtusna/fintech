using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Domain.Enums;
using FluentValidation;

namespace fintech.API.Application.Validators.BudgetDtosValidators
{
    public class BudgetRequestDtoValidator : AbstractValidator<BudgetRequestDto>
    {
        public BudgetRequestDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be a valid enum value");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required for category-specific budgets.")
                .When(x => x.Type == BudgetType.ByCategory);
            RuleFor(x => x.CategoryId)
                .Empty().WithMessage("CategoryId must not be provided for global budgets.")
                .When(x => x.Type == BudgetType.Global);
            RuleFor(x => x.Amount)
                .NotNull().WithMessage("Amount is required.");

            When(x => x.Amount != null, () =>
            {
                RuleFor(x => x.Amount.Amount)
                    .GreaterThan(0).WithMessage("Budget amount must be greater than zero.");

                RuleFor(x => x.Amount.Currency)
                    .NotNull().WithMessage("Currency is required.");
            });
            When(x => x.Period != null, () =>
            {
                RuleFor(x => x.Period.StartDate)
                    .LessThanOrEqualTo(x => x.Period.EndDate)
                    .WithMessage("Period start date cannot be later than the end date.");
            });
        }
    }
}
