using fintech.API.Application.DTOs.ExchangeRateDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.ExchangeRateDtosValidators
{
    public class CreateExchangeRateRequestDtoValidator : AbstractValidator<CreateExchangeRateRequestDto>
    {
        public CreateExchangeRateRequestDtoValidator()
        {
            RuleFor(x => x.FromCurrency)
                .NotEmpty().WithMessage("FromCurrency is required.")
                .Length(3).WithMessage("FromCurrency must be a 3-letter code.")
                .Must(c => c.Equals(c, StringComparison.CurrentCultureIgnoreCase)).WithMessage("FromCurrency must be uppercase.")
                .Matches("^[A-Z]{3}$").WithMessage("FromCurrency must be a valid ISO code (e.g., USD, EUR)");
            RuleFor(x => x.ToCurrency)
                .NotEmpty().WithMessage("ToCurrency is required.")
                .Length(3).WithMessage("ToCurrency must be a 3-letter code.")
                .Must(c => c.Equals(c, StringComparison.CurrentCultureIgnoreCase)).WithMessage("ToCurrency must be uppercase.")
                .Matches("^[A-Z]{3}$").WithMessage("ToCurrency must be a valid ISO code (e.g., USD, EUR)");
            RuleFor(x => x.Rate)
                .GreaterThan(0).WithMessage("Rate must be greater than 0.");
        }
    }
}
