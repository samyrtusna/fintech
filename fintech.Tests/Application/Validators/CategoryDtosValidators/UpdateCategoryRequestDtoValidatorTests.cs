using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.Validators.CategoryDtosValidators;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.CategoryDtosValidators
{
    public class UpdateCategoryRequestDtoValidatorTests
    {
        private readonly UpdateCategoryRequestDtoValidator _validator;

        public UpdateCategoryRequestDtoValidatorTests()
        {
            _validator = new UpdateCategoryRequestDtoValidator();
        }

        [Fact]
        public void ValidUpdateCategoryRequest_ShouldNotHaveValidationErrors()
        {
            var dto = new UpdateCategoryRequestDto
            {
                Name = "Updated Groceries"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyName_ShouldHaveValidationError()
        {
            var dto = new UpdateCategoryRequestDto
            {
                Name = ""
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Category name is required.");
        }

        [Fact]
        public void NameLongerThan100Characters_ShouldHaveValidationError()
        {
            var dto = new UpdateCategoryRequestDto
            {
                Name = new string('a', 101)
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(
                    "Category name must be at most 100 characters long.");
        }
    }
}