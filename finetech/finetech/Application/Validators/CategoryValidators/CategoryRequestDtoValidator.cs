using fintech.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace fintech.Application.Validators.CategoryValidators
{
    public class CategoryRequestDtoValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must be at most 100 characters long.");
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid category type.");
        }
    }
}
