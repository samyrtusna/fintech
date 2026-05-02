using fintech.Application.DTOs.TransactionDtos;
using FluentValidation;

namespace fintech.Application.Validators.TransactionValidators
{
    public class TransactionRequestDtoValidator : AbstractValidator<TransactionRequestDto>
    {
        public TransactionRequestDtoValidator()
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
                .Must(c => c == c.ToUpper()).WithMessage("Currency must be uppercase.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be a valid ISO code (e.g., USD, EUR)");
            RuleFor(x => x.ExchangeRate)
                .GreaterThan(0)
                .WithMessage("Exchange rate must be greater than zero.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
        }
    }
}
