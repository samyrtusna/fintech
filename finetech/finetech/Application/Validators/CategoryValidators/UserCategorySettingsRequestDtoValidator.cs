using fintech.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace fintech.Application.Validators.CategoryValidators
{
    public class UserCategorySettingsRequestDtoValidator : AbstractValidator<UserCategorySettingsRequestDto>
    {
        public UserCategorySettingsRequestDtoValidator()
        {
            RuleFor(x => x.IsEssential)
                .NotNull().WithMessage("IsEssential flag is required.");
        }
    }
}
