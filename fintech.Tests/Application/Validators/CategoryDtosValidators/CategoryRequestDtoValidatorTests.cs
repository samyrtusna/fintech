using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.Validators.CategoryDtosValidators;
using fintech.API.Domain.Enums;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.CategoryDtosValidators
{
    public class CategoryRequestDtoValidatorTests
    {
        private readonly CategoryRequestDtoValidator _validator;

        public CategoryRequestDtoValidatorTests()
        {
            _validator = new CategoryRequestDtoValidator();
        }

        [Fact]
        public void ValidCategoryRequest_ShouldNotHaveValidationErrors()
        {
            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyName_ShouldHaveValidationError()
        {
            var dto = new CategoryRequestDto
            {
                Name = "",
                Type = FinancialType.Expense
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Category name is required.");
        }

        [Fact]
        public void NameLongerThan100Characters_ShouldHaveValidationError()
        {
            var dto = new CategoryRequestDto
            {
                Name = new string('a', 101),
                Type = FinancialType.Expense
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(
                    "Category name must be at most 100 characters long.");
        }

        [Fact]
        public void InvalidCategoryType_ShouldHaveValidationError()
        {
            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = (FinancialType)999
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Type)
                .WithErrorMessage("Invalid category type.");
        }
    }
}