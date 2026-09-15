using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Validators.AuthDtosValidators;
using FluentValidation.TestHelper;

namespace fintech.Tests.Application.Validators.AuthDtosValidators
{
    public class RegisterRequestDtoValidatorTests
    {
        private readonly RegisterRequestDtoValidator _validator;

        public RegisterRequestDtoValidatorTests()
        {
            _validator = new RegisterRequestDtoValidator();
        }

        [Fact]
        public void ValidRegisterRequest_ShouldNotHaveValidationErrors()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "john",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void EmptyEmail_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "",
                Username = "john",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void InvalidEmail_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "invalid-email",
                Username = "john",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void EmptyUsername_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage("Username is required.");
        }

        [Fact]
        public void ShortUsername_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "ab",
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage(
                    "Username must be at least 3 characters long.");
        }

        [Fact]
        public void LongUsername_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = new string('a', 51),
                Password = "Password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage(
                    "Username must be at most 50 characters long.");
        }

        [Fact]
        public void EmptyPassword_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "john",
                Password = ""
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void ShortPassword_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "john",
                Password = "Pass1"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(
                    "Password must be at least 6 characters long.");
        }

        [Fact]
        public void PasswordWithoutUppercase_ShouldHaveValidationError()
        {
            var dto = new RegisterRequestDto
            {
                Email = "user@example.com",
                Username = "john",
                Password = "password123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(
                    "Password must contain at least one uppercase letter");
        }
    }
}