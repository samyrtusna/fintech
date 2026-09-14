using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Services;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using Moq;
using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.Tests.Application.Services
{
    public class FinancialTransactionServiceTests
    {
        private readonly Mock<IFinancialTransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IUserCategorySettingRepository> _categorySettingRepositoryMock;
        private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;

        private readonly FinancialTransactionService _service;

        public FinancialTransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<IFinancialTransactionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categorySettingRepositoryMock = new Mock<IUserCategorySettingRepository>();
            _exchangeRateApiServiceMock = new Mock<IExchangeRateApiService>();

            _service = new FinancialTransactionService(
                _transactionRepositoryMock.Object,
                _userRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _categorySettingRepositoryMock.Object,
                _exchangeRateApiServiceMock.Object);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            FinancialTransactionRequestDto? dto = null;
            var userId = Guid.NewGuid();

            // Act
            var act = () => _service.CreateFinancialTransactionAsync(dto!, userId);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            // Arrange
            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = Guid.NewGuid(),
                Amount = 100,
                Currency = "USD"
            };

            // Act
            var act = () => _service.CreateFinancialTransactionAsync(dto, Guid.Empty);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = Guid.NewGuid(),
                Amount = 100,
                Currency = "USD"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var act = () => _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD"
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act
            var act = () => _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldThrowUnauthorizedException_WhenCategoryBelongsToAnotherUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD"
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = anotherUserId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            var act = () => _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(act);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldUseExchangeRateOne_WhenCurrencyMatchesBaseCurrency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD"
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            FinancialTransaction? addedTransaction = null;

            _transactionRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<FinancialTransaction>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FinancialTransaction, CancellationToken>(
                    (transaction, _) => addedTransaction = transaction)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.NotNull(addedTransaction);
            Assert.Equal(1, addedTransaction.ExchangeRate);
            Assert.Equal(100, addedTransaction.BaseAmount);

            _exchangeRateApiServiceMock.Verify(
                x => x.GetEchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            Assert.Equal(100, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldGetExchangeRate_WhenCurrencyDiffersFromBaseCurrency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "EUR"
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            _exchangeRateApiServiceMock
                .Setup(x => x.GetEchangeRateAsync("EUR", "USD"))
                .ReturnsAsync(1.1m);

            FinancialTransaction? addedTransaction = null;

            _transactionRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<FinancialTransaction>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FinancialTransaction, CancellationToken>(
                    (transaction, _) => addedTransaction = transaction)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.NotNull(addedTransaction);
            Assert.Equal(1.1m, addedTransaction.ExchangeRate);
            Assert.Equal(110m, addedTransaction.BaseAmount);

            _exchangeRateApiServiceMock.Verify(
                x => x.GetEchangeRateAsync("EUR", "USD"),
                Times.Once);

            Assert.Equal(110m, result.BaseAmount);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldUseCategoryType()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 500,
                Currency = "USD"
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Salary",
                Type = FinancialType.Income,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            _transactionRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<FinancialTransaction>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.Equal(FinancialType.Income, result.Type);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldUseDtoIsEssential_WhenProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD",
                IsEssential = true
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.True(result.IsEssential);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldUseCategorySetting_WhenDtoIsEssentialIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD",
                IsEssential = null
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            var categorySetting = new UserCategorySetting
            {
                CategoryId = categoryId,
                UserId = userId,
                IsEssential = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(categorySetting);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.True(result.IsEssential);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldDefaultIsEssentialToFalse_WhenDtoAndCategorySettingAreNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD",
                IsEssential = null
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            Assert.False(result.IsEssential);
        }

        [Fact]
        public async Task CreateFinancialTransactionAsync_ShouldAddAndSaveTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = categoryId,
                Amount = 250,
                Currency = "USD",
                Description = "Groceries",
                IsEssential = true
            };

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                BaseCurrency = "USD"
            };

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _categoryRepositoryMock
                .Setup(x => x.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categorySettingRepositoryMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            _transactionRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<FinancialTransaction>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionRepositoryMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateFinancialTransactionAsync(dto, userId);

            // Assert
            _transactionRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<FinancialTransaction>(t =>
                        t.UserId == userId &&
                        t.CategoryId == categoryId &&
                        t.Amount == 250 &&
                        t.Currency == "USD" &&
                        t.Description == "Groceries" &&
                        t.Type == FinancialType.Expense &&
                        t.ExchangeRate == 1 &&
                        t.BaseAmount == 250 &&
                        t.IsEssential),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _transactionRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Equal(categoryId, result.CategoryId);
        }


        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ShouldThrowArgumentException_WhenTransactionIdIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var act = () => _service.GetFinancialTransactionByIdAsync(
                Guid.Empty,
                userId);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            // Act
            var act = () => _service.GetFinancialTransactionByIdAsync(
                transactionId,
                Guid.Empty);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync((FinancialTransaction?)null);

            // Act
            var act = () => _service.GetFinancialTransactionByIdAsync(
                transactionId,
                userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ShouldThrowUnauthorizedException_WhenTransactionBelongsToAnotherUser()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = anotherUserId,
                CategoryId = Guid.NewGuid(),
                Type = FinancialType.Expense,
                Amount = 100,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 100,
                Description = "Test transaction",
                IsEssential = true,
                TransactionDate = DateTime.UtcNow
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var act = () => _service.GetFinancialTransactionByIdAsync(
                transactionId,
                userId);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ShouldReturnTransaction_WhenUserIsAuthorized()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = userId,
                CategoryId = categoryId,
                Category = category,
                Type = FinancialType.Expense,
                Amount = 150,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 150,
                Description = "Groceries",
                IsEssential = true,
                TransactionDate = DateTime.UtcNow
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var result = await _service.GetFinancialTransactionByIdAsync(
                transactionId,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            Assert.Equal(categoryId, result.CategoryId);
            Assert.Equal(150, result.Amount);
            Assert.Equal("USD", result.Currency);
            Assert.Equal(150, result.BaseAmount);
            Assert.Equal("Groceries", result.Description);
            Assert.True(result.IsEssential);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var act = () => _service.UpdateFinancialTransactionAsync(
                transactionId,
                null!,
                userId);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldThrowArgumentException_WhenTransactionIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated description",
                IsEssential = true
            };

            var act = () => _service.UpdateFinancialTransactionAsync(
                Guid.Empty,
                dto,
                userId);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var transactionId = Guid.NewGuid();

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated description",
                IsEssential = true
            };

            var act = () => _service.UpdateFinancialTransactionAsync(
                transactionId,
                dto,
                Guid.Empty);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated description",
                IsEssential = true
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync((FinancialTransaction?)null);

            var act = () => _service.UpdateFinancialTransactionAsync(
                transactionId,
                dto,
                userId);

            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldThrowUnauthorizedException_WhenTransactionBelongsToAnotherUser()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = anotherUserId,
                CategoryId = Guid.NewGuid(),
                Type = FinancialType.Expense,
                Amount = 100,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 100,
                Description = "Original description",
                IsEssential = false,
                TransactionDate = DateTime.UtcNow
            };

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated description",
                IsEssential = true
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            var act = () => _service.UpdateFinancialTransactionAsync(
                transactionId,
                dto,
                userId);

            await Assert.ThrowsAsync<UnauthorizedException>(act);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ShouldUpdateTransactionAndSaveChanges_WhenUserIsAuthorized()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
                Type = FinancialType.Expense,
                Amount = 100,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 100,
                Description = "Original description",
                IsEssential = false,
                TransactionDate = DateTime.UtcNow
            };

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated description",
                IsEssential = true
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            _transactionRepositoryMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateFinancialTransactionAsync(
                transactionId,
                dto,
                userId);

            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            Assert.Equal("Updated description", result.Description);
            Assert.True(result.IsEssential);

            Assert.Equal("Updated description", transaction.Description);
            Assert.True(transaction.IsEssential);

            _transactionRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ShouldThrowArgumentException_WhenTransactionIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var act = () => _service.DeleteFinancialTransactionAsync(
                Guid.Empty,
                userId);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var transactionId = Guid.NewGuid();

            var act = () => _service.DeleteFinancialTransactionAsync(
                transactionId,
                Guid.Empty);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync((FinancialTransaction?)null);

            var act = () => _service.DeleteFinancialTransactionAsync(
                transactionId,
                userId);

            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ShouldThrowUnauthorizedException_WhenTransactionBelongsToAnotherUser()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = anotherUserId,
                CategoryId = Guid.NewGuid(),
                Type = FinancialType.Expense,
                Amount = 100,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 100,
                Description = "Test transaction",
                IsEssential = true,
                TransactionDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            var act = () => _service.DeleteFinancialTransactionAsync(
                transactionId,
                userId);

            await Assert.ThrowsAsync<UnauthorizedException>(act);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ShouldMarkTransactionAsDeletedAndSaveChanges_WhenUserIsAuthorized()
        {
            var transactionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var transaction = new FinancialTransaction
            {
                Id = transactionId,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
                Type = FinancialType.Expense,
                Amount = 100,
                Currency = "USD",
                ExchangeRate = 1,
                BaseAmount = 100,
                Description = "Test transaction",
                IsEssential = true,
                TransactionDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _transactionRepositoryMock
                .Setup(x => x.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            _transactionRepositoryMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.DeleteFinancialTransactionAsync(
                transactionId,
                userId);

            Assert.True(transaction.IsDeleted);

            _transactionRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            var userId = Guid.NewGuid();

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                null!,
                userId);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var dto = new FinancialTransactionFilterDto();

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                dto,
                Guid.Empty);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowBadRequestException_WhenMonthIsInvalid(
    int month)
        {
            var userId = Guid.NewGuid();

            var dto = new FinancialTransactionFilterDto
            {
                Year = 2026,
                Month = month
            };

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            await Assert.ThrowsAsync<BadRequestException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowBadRequestException_WhenMonthIsProvidedWithoutYear()
        {
            var userId = Guid.NewGuid();

            var dto = new FinancialTransactionFilterDto
            {
                Month = 5
            };

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            await Assert.ThrowsAsync<BadRequestException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowBadRequestException_WhenYearIsBefore2026()
        {
            var userId = Guid.NewGuid();

            var dto = new FinancialTransactionFilterDto
            {
                Year = 2025
            };

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            await Assert.ThrowsAsync<BadRequestException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldThrowBadRequestException_WhenYearIsInTheFuture()
        {
            var userId = Guid.NewGuid();

            var dto = new FinancialTransactionFilterDto
            {
                Year = DateTime.Now.Year + 1
            };

            var act = () => _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            await Assert.ThrowsAsync<BadRequestException>(act);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldReturnOnlyUserTransactions()
        {
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Category = category,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "My transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = anotherUserId,
                    CategoryId = category.Id,
                    Category = category,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Another user's transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 11),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);

            var transaction = result.Items.Single();

            Assert.Equal("My transaction", transaction.Description);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldExcludeDeletedTransactions()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "Active transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Deleted transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 11),
                    IsDeleted = true
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("Active transaction", transaction.Description);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldFilterByYear()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Salary",
                Type = FinancialType.Income,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Income,
                    Amount = 1000,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 1000,
                    Description = "2026 income",
                    IsEssential = false,
                    TransactionDate = new DateTime(2026, 5, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Income,
                    Amount = 900,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 900,
                    Description = "2025 income",
                    IsEssential = false,
                    TransactionDate = new DateTime(2025, 5, 10),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Year = 2026,
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("2026 income", transaction.Description);
            Assert.Equal(1000, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldFilterByMonth()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "September transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "August transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 8, 10),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Year = 2026,
                Month = 9,
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("September transaction", transaction.Description);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldFilterByDay()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "September 14 transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 14),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "September 15 transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 15),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Year = 2026,
                Month = 9,
                Day = 14,
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("September 14 transaction", transaction.Description);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldFilterByCategory()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var groceriesCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            var transportCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Transport",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.AddRange(
                groceriesCategory,
                transportCategory);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = groceriesCategory.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "Groceries transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = transportCategory.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Transport transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 11),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                CategoryId = groceriesCategory.Id,
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("Groceries transaction", transaction.Description);
            Assert.Equal(groceriesCategory.Id, transaction.CategoryId);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldFilterByIsEssential()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "Essential transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Non-essential transaction",
                    IsEssential = false,
                    TransactionDate = new DateTime(2026, 9, 11),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                IsEssential = true,
                Page = 1,
                PageSize = 10
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            var transaction = result.Items.Single();

            Assert.Equal("Essential transaction", transaction.Description);
            Assert.True(transaction.IsEssential);
            Assert.Equal(100, transaction.Amount);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldOrderByDateDescendingAndApplyPagination()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "Oldest transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Middle transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 15),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 300,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 300,
                    Description = "Newest transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 20),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Page = 1,
                PageSize = 2
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            var items = result.Items.ToList();

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.PageSize);

            Assert.Equal(2, items.Count);

            Assert.Equal("Newest transaction", items[0].Description);
            Assert.Equal("Middle transaction", items[1].Description);
        }

        [Fact]
        public async Task GetFinancialTransactionsByFilterAsync_ShouldReturnCorrectItemsForSecondPage()
        {
            var userId = Guid.NewGuid();

            await using var context = CreateInMemoryContext();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            context.Categories.Add(category);

            context.FinancialTransactions.AddRange(
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 100,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 100,
                    Description = "Oldest transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 10),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 200,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 200,
                    Description = "Middle transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 15),
                    IsDeleted = false
                },
                new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.Id,
                    Type = FinancialType.Expense,
                    Amount = 300,
                    Currency = "USD",
                    ExchangeRate = 1,
                    BaseAmount = 300,
                    Description = "Newest transaction",
                    IsEssential = true,
                    TransactionDate = new DateTime(2026, 9, 20),
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            _transactionRepositoryMock
                .Setup(x => x.Query())
                .Returns(context.FinancialTransactions);

            var dto = new FinancialTransactionFilterDto
            {
                Page = 2,
                PageSize = 2
            };

            var result = await _service.GetFinancialTransactionsByFilterAsync(
                dto,
                userId);

            var items = result.Items.ToList();

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.PageSize);

            Assert.Single(items);

            Assert.Equal("Oldest transaction", items[0].Description);
            Assert.Equal(100, items[0].Amount);
        }

        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
