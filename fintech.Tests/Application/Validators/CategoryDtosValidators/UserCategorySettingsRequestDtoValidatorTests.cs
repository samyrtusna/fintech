using fintech.API.Application.DTOs.UserCategorySettingDtos;
using fintech.API.Application.Validators.CategoryDtosValidators;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.CategoryDtosValidators
{
    public class UserCategorySettingsRequestDtoValidatorTests
    {
        private readonly UserCategorySettingsRequestDtoValidator _validator;

        public UserCategorySettingsRequestDtoValidatorTests()
        {
            _validator = new UserCategorySettingsRequestDtoValidator();
        }

        [Fact]
        public void IsEssentialTrue_ShouldNotHaveValidationErrors()
        {
            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.IsEssential);
        }

        [Fact]
        public void IsEssentialFalse_ShouldNotHaveValidationErrors()
        {
            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = false
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.IsEssential);
        }
    }
}