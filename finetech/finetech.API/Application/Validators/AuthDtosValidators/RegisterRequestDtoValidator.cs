using fintech.API.Application.DTOs.AuthDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.AuthDtosValidators
{
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator() { 
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username must be at most 50 characters long.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter");
            RuleFor(x => x.BaseCurrency)
                .NotEmpty().WithMessage("Base currency is required.")
                .Length(3).WithMessage("Base currency must be a 3-letter code.")
                .Must(c => c.Equals(c, StringComparison.CurrentCultureIgnoreCase)).WithMessage("Base currency must be uppercase.")
                .Matches("^[A-Z]{3}$").WithMessage("Base currency must be a valid ISO code (e.g., USD, EUR)"); 
        }
    }
}
