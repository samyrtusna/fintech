using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.DTOs.UserCategorySettingDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Services;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using Moq;

namespace fintech.Tests.Application.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IUserCategorySettingRepository> _userCategorySettingsRepositoryMock;
        private readonly Mock<IFinancialTransactionRepository> _transactionRepositoryMock;

        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _userCategorySettingsRepositoryMock = new Mock<IUserCategorySettingRepository>();
            _transactionRepositoryMock = new Mock<IFinancialTransactionRepository>();

            _service = new CategoryService(
                _categoryRepositoryMock.Object,
                _userCategorySettingsRepositoryMock.Object,
                _transactionRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateCategoryAsync_NullDto_ThrowsArgumentNullException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.CreateCategoryAsync(null!, userId));
        }

        [Fact]
        public async Task CreateCategoryAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var dto = new CategoryRequestDto
            {
                Name = "Food",
                Type = FinancialType.Expense
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateCategoryAsync(dto, Guid.Empty));
        }

        [Fact]
        public async Task CreateCategoryAsync_ParentCategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var parentCategoryId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = parentCategoryId
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(parentCategoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CreateCategoryAsync(dto, userId));
        }

        [Fact]
        public async Task CreateCategoryAsync_ParentCategoryBelongsToAnotherUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var parentCategoryId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = parentCategoryId
            };

            var parentCategory = new Category
            {
                Id = parentCategoryId,
                UserId = otherUserId,
                Name = "Other User Category",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(parentCategoryId))
                .ReturnsAsync(parentCategory);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.CreateCategoryAsync(dto, userId));
        }

        [Fact]
        public async Task CreateCategoryAsync_CategoryAlreadyExists_ThrowsBadRequestException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Food",
                Type = FinancialType.Expense
            };

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateCategoryAsync(dto, userId));
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidCategory_CreatesCategoryAndReturnsResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Food",
                Type = FinancialType.Expense
            };

            Category? addedCategory = null;

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Category, CancellationToken>((category, _) =>
                {
                    addedCategory = category;
                })
                .Returns(Task.CompletedTask);

            _categoryRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCategoryAsync(dto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(addedCategory);

            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Type, result.Type);
            Assert.Equal(dto.ParentCategoryId, result.ParentCategoryId);

            Assert.Equal(userId, addedCategory!.UserId);

            _categoryRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_SystemParentCategory_CreatesCategory()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var parentCategoryId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = parentCategoryId
            };

            var systemParent = new Category
            {
                Id = parentCategoryId,
                Name = "Expenses",
                Type = FinancialType.Expense,
                IsSystem = true,
                UserId = null
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(parentCategoryId))
                .ReturnsAsync(systemParent);

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _categoryRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCategoryAsync(dto, userId);

            // Assert
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Type, result.Type);
            Assert.Equal(parentCategoryId, result.ParentCategoryId);

            _categoryRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_UserOwnedParentCategory_CreatesCategory()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var parentCategoryId = Guid.NewGuid();

            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = parentCategoryId
            };

            var parentCategory = new Category
            {
                Id = parentCategoryId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false,
                UserId = userId
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(parentCategoryId))
                .ReturnsAsync(parentCategory);

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _categoryRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCategoryAsync(dto, userId);

            // Assert
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Type, result.Type);
            Assert.Equal(parentCategoryId, result.ParentCategoryId);

            _categoryRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetAllCategoriesAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ValidUserId_ReturnsMappedCategories()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var categories = new List<Category>
    {
        new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Food",
            Type = FinancialType.Expense
        },
        new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Salary",
            Type = FinancialType.Income
        }
    };

            _categoryRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllCategoriesAsync(userId);

            // Assert
            var resultList = result.ToList();

            Assert.NotNull(result);
            Assert.Equal(2, resultList.Count);

            Assert.Equal(categories[0].Id, resultList[0].Id);
            Assert.Equal(categories[0].Name, resultList[0].Name);
            Assert.Equal(categories[0].Type, resultList[0].Type);
            Assert.Equal(categories[0].ParentCategoryId, resultList[0].ParentCategoryId);

            Assert.Equal(categories[1].Id, resultList[1].Id);
            Assert.Equal(categories[1].Name, resultList[1].Name);
            Assert.Equal(categories[1].Type, resultList[1].Type);
            Assert.Equal(categories[1].ParentCategoryId, resultList[1].ParentCategoryId);

            _categoryRepositoryMock.Verify(
                r => r.GetByUserAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_NoCategories_ReturnsEmptyCollection()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync([]);

            // Act
            var result = await _service.GetAllCategoriesAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _categoryRepositoryMock.Verify(
                r => r.GetByUserAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_EmptyCategoryId_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetCategoryByIdAsync(Guid.Empty, userId));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetCategoryByIdAsync(categoryId, Guid.Empty));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_CategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryByIdAsync(categoryId, userId));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_CategoryBelongsToAnotherUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = otherUserId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.GetCategoryByIdAsync(categoryId, userId));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_SystemCategory_ReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = null,
                Name = "Expenses",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            var result = await _service.GetCategoryByIdAsync(categoryId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Type, result.Type);
            Assert.Equal(category.ParentCategoryId, result.ParentCategoryId);

            _categoryRepositoryMock.Verify(
                r => r.GetAsync(categoryId),
                Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_OwnCategory_ReturnsMappedCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var parentCategoryId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = parentCategoryId,
                IsSystem = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            var result = await _service.GetCategoryByIdAsync(categoryId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Type, result.Type);
            Assert.Equal(category.ParentCategoryId, result.ParentCategoryId);

            _categoryRepositoryMock.Verify(
                r => r.GetAsync(categoryId),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_NullDto_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.UpdateCategoryAsync(categoryId, null!, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_EmptyCategoryId_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateCategoryAsync(Guid.Empty, dto, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateCategoryAsync(categoryId, dto, Guid.Empty));
        }

        [Fact]
        public async Task UpdateCategoryAsync_CategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateCategoryAsync(categoryId, dto, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_SystemCategory_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                Name = "Expenses",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            var dto = new UpdateCategoryRequestDto
            {
                Name = "My Expenses"
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdateCategoryAsync(categoryId, dto, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_CategoryBelongsToAnotherUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = otherUserId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdateCategoryAsync(categoryId, dto, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_NameAlreadyExists_ThrowsBadRequestException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.UpdateCategoryAsync(categoryId, dto, userId));
        }

        [Fact]
        public async Task UpdateCategoryAsync_ValidRequest_UpdatesCategoryAndReturnsResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Groceries"
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.CategoryExistAsync(dto.Name, userId))
                .ReturnsAsync(false);

            _categoryRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateCategoryAsync(
                categoryId,
                dto,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Groceries", result.Name);
            Assert.Equal(FinancialType.Expense, result.Type);

            Assert.Equal("Groceries", category.Name);

            _categoryRepositoryMock.Verify(
                r => r.GetAsync(categoryId),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.CategoryExistAsync(dto.Name, userId),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_EmptyCategoryId_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteCategoryAsync(Guid.Empty, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteCategoryAsync(categoryId, Guid.Empty));
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeleteCategoryAsync(categoryId, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_SystemCategory_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                Name = "Expenses",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.DeleteCategoryAsync(categoryId, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryBelongsToAnotherUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = otherUserId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.DeleteCategoryAsync(categoryId, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryHasChildCategories_ThrowsBadRequestException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var childCategory = new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Groceries",
                Type = FinancialType.Expense,
                ParentCategoryId = categoryId,
                IsSystem = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.GetSubCategoriesAsync(categoryId, userId))
                .ReturnsAsync([childCategory]);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.DeleteCategoryAsync(categoryId, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryUsedInTransaction_ThrowsBadRequestException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var transaction = new FinancialTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = categoryId,
                Amount = 100,
                Currency = "USD",
                BaseAmount = 100,
                Type = FinancialType.Expense
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.GetSubCategoriesAsync(categoryId, userId))
                .ReturnsAsync([]);

            _transactionRepositoryMock
                .Setup(r => r.GetByCategoryAsync(categoryId))
                .ReturnsAsync([transaction]);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.DeleteCategoryAsync(categoryId, userId));
        }

        [Fact]
        public async Task DeleteCategoryAsync_ValidCategory_SoftDeletesAndSaves()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false,
                IsDeleted = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.GetSubCategoriesAsync(categoryId, userId))
                .ReturnsAsync([]);

            _transactionRepositoryMock
                .Setup(r => r.GetByCategoryAsync(categoryId))
                .ReturnsAsync([]);

            _categoryRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteCategoryAsync(categoryId, userId);

            // Assert
            Assert.True(category.IsDeleted);

            _categoryRepositoryMock.Verify(
                r => r.GetAsync(categoryId),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.GetSubCategoriesAsync(categoryId, userId),
                Times.Once);

            _transactionRepositoryMock.Verify(
                r => r.GetByCategoryAsync(categoryId),
                Times.Once);

            _categoryRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_NullDto_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.SetCategoryEssentialAsync(null!, categoryId, userId));
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_EmptyCategoryId_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SetCategoryEssentialAsync(dto, Guid.Empty, userId));
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SetCategoryEssentialAsync(dto, categoryId, Guid.Empty));
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_CategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.SetCategoryEssentialAsync(dto, categoryId, userId));
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_CategoryBelongsToAnotherUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = otherUserId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.SetCategoryEssentialAsync(dto, categoryId, userId));
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_SystemCategory_UpdatesSetting()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                Name = "Expenses",
                Type = FinancialType.Expense,
                IsSystem = true
            };

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            var existingSetting = new UserCategorySetting
            {
                UserId = userId,
                CategoryId = categoryId,
                IsEssential = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.GetByUserAsync(userId, categoryId))
                .ReturnsAsync(existingSetting);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SetCategoryEssentialAsync(
                dto,
                categoryId,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(existingSetting.IsEssential);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.Update(existingSetting),
                Times.Once);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_ExistingSetting_UpdatesSetting()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            var existingSetting = new UserCategorySetting
            {
                UserId = userId,
                CategoryId = categoryId,
                IsEssential = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.GetByUserAsync(userId, categoryId))
                .ReturnsAsync(existingSetting);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SetCategoryEssentialAsync(
                dto,
                categoryId,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(existingSetting.IsEssential);

            Assert.Contains(category.Name, result.Message);
            Assert.Contains("essential", result.Message);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.Update(existingSetting),
                Times.Once);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<UserCategorySetting>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_NoExistingSetting_CreatesSetting()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Food",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            UserCategorySetting? addedSetting = null;

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.GetByUserAsync(userId, categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<UserCategorySetting>(),
                    It.IsAny<CancellationToken>()))
                .Callback<UserCategorySetting, CancellationToken>((setting, _) =>
                {
                    addedSetting = setting;
                })
                .Returns(Task.CompletedTask);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SetCategoryEssentialAsync(
                dto,
                categoryId,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(addedSetting);

            Assert.Equal(userId, addedSetting!.UserId);
            Assert.Equal(categoryId, addedSetting.CategoryId);
            Assert.True(addedSetting.IsEssential);

            Assert.Contains(category.Name, result.Message);
            Assert.Contains("essential", result.Message);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<UserCategorySetting>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _userCategorySettingsRepositoryMock.Verify(
                r => r.Update(It.IsAny<UserCategorySetting>()),
                Times.Never);
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_SetNonEssential_ReturnsCorrectConfirmation()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                UserId = userId,
                Name = "Entertainment",
                Type = FinancialType.Expense,
                IsSystem = false
            };

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(categoryId))
                .ReturnsAsync(category);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.GetByUserAsync(userId, categoryId))
                .ReturnsAsync((UserCategorySetting?)null);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<UserCategorySetting>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userCategorySettingsRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SetCategoryEssentialAsync(
                dto,
                categoryId,
                userId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(category.Name, result.Message);
            Assert.Contains("non-essential", result.Message);
        }
    }
}