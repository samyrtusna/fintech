using fintech.API.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.CategoryDtosValidators
{
    public class UpdateCategoryRequestDtoValidator : AbstractValidator<UpdateCategoryRequestDto>
    {
        public UpdateCategoryRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must be at most 100 characters long.");
        }
    }
}
