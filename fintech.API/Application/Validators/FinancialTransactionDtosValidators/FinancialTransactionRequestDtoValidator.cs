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
                .NotEmpty().WithMessage("Amount is required");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters long.");
        }
    }
}
//TODO: Review the Validation rules for the entire application.