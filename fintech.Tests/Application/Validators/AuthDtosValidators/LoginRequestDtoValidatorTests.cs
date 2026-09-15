using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Validators.AuthDtosValidators;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.AuthDtosValidators
{
    public class LoginRequestDtoValidatorTests
    {
        private readonly LoginRequestDtoValidator _validator;

        public LoginRequestDtoValidatorTests()
        {
            _validator = new LoginRequestDtoValidator();
        }

        [Fact]
        public void ValidLoginRequest_ShouldNotHaveValidationErrors()
        {
            var dto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyEmail_ShouldHaveValidationError()
        {
            var dto = new LoginRequestDto
            {
                Email = "",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void InvalidEmail_ShouldHaveValidationError()
        {
            var dto = new LoginRequestDto
            {
                Email = "invalid-email",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void EmptyPassword_ShouldHaveValidationError()
        {
            var dto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = ""
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void ShortPassword_ShouldHaveValidationError()
        {
            var dto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "Pass1"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password must be at least 6 characters long.");
        }

        [Fact]
        public void PasswordWithoutUppercase_ShouldHaveValidationError()
        {
            var dto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(
                    "Password must contain at least one uppercase letter");
        }
    }
}