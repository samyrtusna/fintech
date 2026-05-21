using fintech.API.Application.DTOs.FinancialIndicatorsDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.FinancialIndicatorDtosValidators
{
    public class FinancialIndicatorRequestDtoValidator : AbstractValidator<FinancialIndicatorsRequestDto>
    {
        public FinancialIndicatorRequestDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.SnapshotId).NotEmpty().WithMessage("SnapshotId is required.");
            RuleFor(x => x.PeriodStart).NotEmpty().WithMessage("PeriodStart is required.")
                .LessThanOrEqualTo(x => x.PeriodEnd).WithMessage("PeriodStart must be less than or equal to PeriodEnd.");
            RuleFor(x => x.PeriodEnd).NotEmpty().WithMessage("PeriodEnd is required.")
                .GreaterThanOrEqualTo(x => x.PeriodStart).WithMessage("PeriodEnd must be greater than or equal to PeriodStart.");
            RuleFor(x => x.PeriodType).NotEmpty().WithMessage("PeriodType is required.")
                .IsInEnum().WithMessage("Invalid PeriodType type.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters long.");
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
        }
    }
}
