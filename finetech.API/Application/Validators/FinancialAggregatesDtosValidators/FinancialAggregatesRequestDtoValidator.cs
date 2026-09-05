using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.FinancialAggregatesDtosValidators
{
    public class FinancialAggregatesRequestDtoValidator : AbstractValidator<FinancialAggregatesRequestDto>
    {
        public FinancialAggregatesRequestDtoValidator()
        {
            RuleFor(x => x.Year).NotEmpty().WithMessage("Year is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
        }
    }
}
