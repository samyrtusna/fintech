using fintech.API.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.CategoryDtosValidators
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
