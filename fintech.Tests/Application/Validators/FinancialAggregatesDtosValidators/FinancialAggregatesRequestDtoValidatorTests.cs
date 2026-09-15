using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.Validators.FinancialAggregatesDtosValidators;
using fintech.API.Domain.Enums;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.FinancialAggregatesDtosValidators
{
    public class FinancialAggregatesRequestDtoValidatorTests
    {
        private readonly FinancialAggregatesRequestDtoValidator _validator;

        public FinancialAggregatesRequestDtoValidatorTests()
        {
            _validator = new FinancialAggregatesRequestDtoValidator();
        }

        [Fact]
        public void ValidFinancialAggregatesRequest_ShouldNotHaveValidationErrors()
        {
            var dto = new FinancialAggregatesRequestDto
            {
                Year = 2026,
                Name = (AggregateName)1,
                Value = 1500m
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyYear_ShouldHaveValidationError()
        {
            var dto = new FinancialAggregatesRequestDto
            {
                Year = 0,
                Name = (AggregateName)1,
                Value = 1500m
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Year)
                .WithErrorMessage("Year is required.");
        }

        [Fact]
        public void EmptyName_ShouldHaveValidationError()
        {
            var dto = new FinancialAggregatesRequestDto
            {
                Year = 2026,
                Name = default,
                Value = 1500m
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void EmptyValue_ShouldHaveValidationError()
        {
            var dto = new FinancialAggregatesRequestDto
            {
                Year = 2026,
                Name = (AggregateName)1,
                Value = 0m
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Value)
                .WithErrorMessage("Value is required.");
        }
    }
}