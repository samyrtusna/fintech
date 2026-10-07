using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Services;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace fintech.Tests.Application.Services
{
    public class FinancialTransactionServiceTests
    {
        private readonly Mock<IFinancialTransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IUserCategorySettingRepository> _categorySettingRepositoryMock;
        private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
        private readonly Mock<ILogger<FinancialTransactionService>> _loggerMock;
        private readonly Mock<IUserCurrenciesRepositoy> _userCurrenciesRepositoryMock;

        private readonly FinancialTransactionService _service;

        public FinancialTransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<IFinancialTransactionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categorySettingRepositoryMock = new Mock<IUserCategorySettingRepository>();
            _exchangeRateApiServiceMock = new Mock<IExchangeRateApiService>();
            _loggerMock = new Mock<ILogger<FinancialTransactionService>>();
            _userCurrenciesRepositoryMock = new Mock<IUserCurrenciesRepositoy>();

            _service = new FinancialTransactionService(
                _transactionRepositoryMock.Object,
                _userRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _categorySettingRepositoryMock.Object,
                _exchangeRateApiServiceMock.Object,
                _loggerMock.Object,
                _userCurrenciesRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldUseExchangeRateOne_WhenTransactionCurrencyMatchesBaseCurrency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashed-password"
            };

            var category = new Category
            {
                Id = categoryId,
                Name = "Salary",
                Type = FinancialType.Income,
                IsSystem = true
            };

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 1000m,
                Currency = "USD",
                Description = "Monthly salary",
                IsEssential = false
            };

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            // FIX: Pass 'default' explicitly to avoid CS0854 on optional arguments
            _userCurrenciesRepositoryMock
                .Setup(x => x.GetDefaultCurrencyAsync(userId, default))
                .ReturnsAsync(new UserCurrency
                {
                    CurrencyCode = "USD",
                    CurrencySymbol = "$"
                });

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000m, result.Amount);
            Assert.Equal(1m, result.ExchangeRate);
            Assert.Equal(1000m, result.BaseAmount);

            _exchangeRateApiServiceMock.Verify(
                x => x.GetEchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            // FIX: Pass 'default' explicitly here as well
            _transactionRepositoryMock.Verify(
                x => x.SaveChangesAsync(default),
                Times.Once);
        }
    }
}