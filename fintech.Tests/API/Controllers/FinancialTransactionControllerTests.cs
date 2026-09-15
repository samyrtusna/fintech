using fintech.API.API.Controllers;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace fintech.Tests.API.Controllers
{
    public class FinancialTransactionControllerTests
    {
        private readonly Mock<IFinancialTransactionService> _financialTransactionServiceMock;
        private readonly FinancialTransactionController _controller;
        private readonly Guid _userId;

        public FinancialTransactionControllerTests()
        {
            _financialTransactionServiceMock =
                new Mock<IFinancialTransactionService>();

            _controller = new FinancialTransactionController(
                _financialTransactionServiceMock.Object);

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
        public async Task CreateFinancialTransactionAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new FinancialTransactionRequestDto
            {
                CategoryId = Guid.NewGuid(),
                Amount = 100,
                Currency = "DZD",
                Description = "Groceries"
            };

            var transaction = new FinancialTransactionResponseDto
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                Currency = "DZD",
                Description = "Groceries"
            };

            _financialTransactionServiceMock
                .Setup(x => x.CreateFinancialTransactionAsync(dto, _userId))
                .ReturnsAsync(transaction);

            var result = await _controller.CreateFinancialTransactionAsync(dto);

            var createdResult =
                Assert.IsType<CreatedAtRouteResult>(result);

            Assert.Equal("GetTransactionById", createdResult.RouteName);
            Assert.Equal(transaction, createdResult.Value);
            Assert.Equal(
                transaction.Id,
                createdResult.RouteValues!["id"]);

            _financialTransactionServiceMock.Verify(
                x => x.CreateFinancialTransactionAsync(dto, _userId),
                Times.Once);
        }

        [Fact]
        public async Task GetFilteredFinancialTransactionsAsync_ValidFilter_ReturnsOkWithResult()
        {
            var dto = new FinancialTransactionFilterDto
            {
                Year = 2026,
                Month = 9,
                Page = 1,
                PageSize = 10
            };

            var paginatedResult =
                new PaginatedResult<FinancialTransactionResponseDto>
                {
                    Items = new List<FinancialTransactionResponseDto>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 10
                };

            _financialTransactionServiceMock
                .Setup(x => x.GetFinancialTransactionsByFilterAsync(dto, _userId))
                .ReturnsAsync(paginatedResult);

            var result =
                await _controller.GetFilteredFinancialTransactionsAsync(dto);

            var okResult =
                Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(paginatedResult, okResult.Value);

            _financialTransactionServiceMock.Verify(
                x => x.GetFinancialTransactionsByFilterAsync(dto, _userId),
                Times.Once);
        }

        [Fact]
        public async Task GetFinancialTransactionByIdAsync_ValidId_ReturnsOkWithTransaction()
        {
            var transactionId = Guid.NewGuid();

            var transaction = new FinancialTransactionResponseDto
            {
                Id = transactionId,
                Amount = 100,
                Currency = "DZD",
                Description = "Groceries"
            };

            _financialTransactionServiceMock
                .Setup(x => x.GetFinancialTransactionByIdAsync(
                    transactionId,
                    _userId))
                .ReturnsAsync(transaction);

            var result =
                await _controller.GetFinancialTransactionByIdAsync(transactionId);

            var okResult =
                Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(transaction, okResult.Value);

            _financialTransactionServiceMock.Verify(
                x => x.GetFinancialTransactionByIdAsync(
                    transactionId,
                    _userId),
                Times.Once);
        }

        [Fact]
        public async Task UpdateFinancialTransactionAsync_ValidRequest_ReturnsOkWithTransaction()
        {
            var transactionId = Guid.NewGuid();

            var dto = new UpdateFinancialTransactionRequestDto
            {
                Description = "Updated groceries",
                IsEssential = true
            };

            var transaction = new FinancialTransactionResponseDto
            {
                Id = transactionId,
                Amount = 100,
                Currency = "DZD",
                Description = "Updated groceries",
                IsEssential = true
            };

            _financialTransactionServiceMock
                .Setup(x => x.UpdateFinancialTransactionAsync(
                    transactionId,
                    dto,
                    _userId))
                .ReturnsAsync(transaction);

            var result =
                await _controller.UpdateFinancialTransactionAsync(
                    transactionId,
                    dto);

            var okResult =
                Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(transaction, okResult.Value);

            _financialTransactionServiceMock.Verify(
                x => x.UpdateFinancialTransactionAsync(
                    transactionId,
                    dto,
                    _userId),
                Times.Once);
        }

        [Fact]
        public async Task DeleteFinancialTransactionAsync_ValidId_ReturnsNoContent()
        {
            var transactionId = Guid.NewGuid();

            _financialTransactionServiceMock
                .Setup(x => x.DeleteFinancialTransactionAsync(
                    transactionId,
                    _userId))
                .Returns(Task.CompletedTask);

            var result =
                await _controller.DeleteFinancialTransactionAsync(transactionId);

            var noContentResult =
                Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, noContentResult.StatusCode);

            _financialTransactionServiceMock.Verify(
                x => x.DeleteFinancialTransactionAsync(
                    transactionId,
                    _userId),
                Times.Once);
        }
    }
}