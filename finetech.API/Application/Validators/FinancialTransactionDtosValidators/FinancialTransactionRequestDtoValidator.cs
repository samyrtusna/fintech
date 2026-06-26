using fintech.API.Application.DTOs.FinancialTransactionDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.FinancialTransactionDtosValidators 
{
    public class FinancialTransactionRequestDtoValidator : AbstractValidator<FinancialTransactionRequestDto>
    {
        public FinancialTransactionRequestDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required");
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must have a positive value")
                .PrecisionScale(18, 2, true)
                .WithMessage("Amount must have max 2 decimal places.");
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter code.")
                .Must(c => c.Equals(c, StringComparison.CurrentCultureIgnoreCase)).WithMessage("Currency must be uppercase.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be a valid ISO code (e.g., USD, EUR)");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
        }
    }
}
