using fintech.API.API.Controllers;
using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.DTOs.UserCategorySettingDtos;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace fintech.Tests.API.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly CategoryController _controller;
        private readonly Guid _userId;

        public CategoryControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _controller = new CategoryController(
                _categoryServiceMock.Object);

            _userId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    _userId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            // Arrange
            var dto = new CategoryRequestDto
            {
                Name = "Groceries",
                Type = FinancialType.Expense
            };

            var category = new CategoryResponseDto
            {
                Id = Guid.NewGuid(),
                Name = "Groceries",
                Type = FinancialType.Expense
            };

            _categoryServiceMock
                .Setup(x => x.CreateCategoryAsync(dto, _userId))
                .ReturnsAsync(category);

            // Act
            var result = await _controller.CreateCategoryAsync(dto);

            // Assert
            var createdResult =
                Assert.IsType<CreatedAtRouteResult>(result.Result);

            Assert.Equal("GetCategoryById", createdResult.RouteName);
            Assert.Equal(category, createdResult.Value);

            Assert.Equal(
                category.Id,
                createdResult.RouteValues!["id"]);

            _categoryServiceMock.Verify(
                x => x.CreateCategoryAsync(dto, _userId),
                Times.Once);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsOkWithCategories()
        {
            // Arrange
            var categories = new List<CategoryResponseDto>
    {
        new CategoryResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Salary",
            Type = FinancialType.Income
        },
        new CategoryResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Groceries",
            Type = FinancialType.Expense
        }
    };

            _categoryServiceMock
                .Setup(x => x.GetAllCategoriesAsync(_userId))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllCategoriesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(categories, okResult.Value);

            _categoryServiceMock.Verify(
                x => x.GetAllCategoriesAsync(_userId),
                Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsOkWithCategory()
        {
            var categoryId = Guid.NewGuid();

            var category = new CategoryResponseDto
            {
                Id = categoryId,
                Name = "Groceries",
                Type = FinancialType.Expense
            };

            _categoryServiceMock
                .Setup(x => x.GetCategoryByIdAsync(categoryId, _userId))
                .ReturnsAsync(category);

            var result = await _controller.GetCategoryByIdAsync(categoryId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(category, okResult.Value);

            _categoryServiceMock.Verify(
                x => x.GetCategoryByIdAsync(categoryId, _userId),
                Times.Once);
        }

        [Fact]
        public async Task SetCategoryEssentialAsync_ValidRequest_ReturnsOkWithConfirmation()
        {
            var categoryId = Guid.NewGuid();

            var dto = new UserCategorySettingsRequestDto
            {
                IsEssential = true
            };

            var confirmation = new ConfirmationResponseDto
            {
                Message = "Category 'Groceries' has been marked as essential."
            };

            _categoryServiceMock
                .Setup(x => x.SetCategoryEssentialAsync(dto, categoryId, _userId))
                .ReturnsAsync(confirmation);

            var result = await _controller.SetCategoryEssentialAsync(
                categoryId,
                dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(confirmation, okResult.Value);

            _categoryServiceMock.Verify(
                x => x.SetCategoryEssentialAsync(dto, categoryId, _userId),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ValidRequest_ReturnsOkWithCategory()
        {
            var categoryId = Guid.NewGuid();

            var dto = new UpdateCategoryRequestDto
            {
                Name = "Food"
            };

            var category = new CategoryResponseDto
            {
                Id = categoryId,
                Name = "Food",
                Type = FinancialType.Expense
            };

            _categoryServiceMock
                .Setup(x => x.UpdateCategoryAsync(
                    categoryId,
                    dto,
                    _userId))
                .ReturnsAsync(category);

            var result = await _controller.UpdateCategoryAsync(
                categoryId,
                dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(category, okResult.Value);

            _categoryServiceMock.Verify(
                x => x.UpdateCategoryAsync(
                    categoryId,
                    dto,
                    _userId),
                Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ValidId_ReturnsNoContent()
        {
            var categoryId = Guid.NewGuid();

            _categoryServiceMock
                .Setup(x => x.DeleteCategoryAsync(categoryId, _userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteCategoryAsync(categoryId);

            Assert.IsType<NoContentResult>(result);

            var noContentResult = (NoContentResult)result;

            Assert.Equal(204, noContentResult.StatusCode);

            _categoryServiceMock.Verify(
                x => x.DeleteCategoryAsync(categoryId, _userId),
                Times.Once);
        }
    }
}