using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Validators.FinancialTransactionDtosValidators;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.FinancialTransactionDtosValidators
{
    public class FinancialTransactionRequestDtoValidatorTests
    {
        private readonly FinancialTransactionRequestDtoValidator _validator;

        public FinancialTransactionRequestDtoValidatorTests()
        {
            _validator = new FinancialTransactionRequestDtoValidator();
        }

        private static FinancialTransactionRequestDto CreateValidDto()
        {
            return new FinancialTransactionRequestDto
            {
                CategoryId = Guid.NewGuid(),
                Amount = 1500.50m,
                Currency = "USD",
                Description = "Grocery shopping"
            };
        }

        [Fact]
        public void ValidFinancialTransactionRequest_ShouldNotHaveValidationErrors()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyCategoryId_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.CategoryId = Guid.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId)
                .WithErrorMessage("Category ID is required");
        }

        [Fact]
        public void ZeroAmount_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Amount = 0m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must have a positive value");
        }

        [Fact]
        public void NegativeAmount_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Amount = -100m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must have a positive value");
        }

        [Fact]
        public void AmountWithMoreThanTwoDecimalPlaces_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Amount = 100.123m;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must have max 2 decimal places.");
        }

        [Fact]
        public void AmountWithTwoDecimalPlaces_ShouldNotHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Amount = 100.12m;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void EmptyCurrency_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Currency = "";

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Currency);
        }

        [Fact]
        public void CurrencyWithLessThanThreeCharacters_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Currency = "US";

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Currency)
                .WithErrorMessage("Currency must be a 3-letter code.");
        }

        [Fact]
        public void CurrencyWithMoreThanThreeCharacters_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Currency = "USDX";

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Currency)
                .WithErrorMessage("Currency must be a 3-letter code.");
        }

        [Fact]
        public void LowercaseCurrency_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Currency = "usd";

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Currency)
                .WithErrorMessage(
                    "Currency must be a valid ISO code (e.g., USD, EUR)");
        }

        [Fact]
        public void InvalidCurrencyFormat_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Currency = "12$";

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Currency)
                .WithErrorMessage("Currency must be a valid ISO code (e.g., USD, EUR)");
        }

        [Fact]
        public void DescriptionLongerThan500Characters_ShouldHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Description = new string('a', 501);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must be at most 500 characters long.");
        }

        [Fact]
        public void DescriptionWith500Characters_ShouldNotHaveValidationError()
        {
            var dto = CreateValidDto();
            dto.Description = new string('a', 500);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
    }
}