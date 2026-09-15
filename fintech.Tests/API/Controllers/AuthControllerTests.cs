using fintech.API.API.Controllers;
using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace fintech.Tests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();

            _controller = new AuthController(
                _authServiceMock.Object,
                _refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidDto_ReturnsOkWithAccessToken()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!"
            };

            const string accessToken = "access-token";

            _authServiceMock
                .Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(accessToken);

            // Act
            var result = await _controller.RegisterAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(accessToken, okResult.Value);

            _authServiceMock.Verify(
                x => x.RegisterAsync(dto),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ValidDto_ReturnsOkWithAccessToken()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            const string accessToken = "access-token";

            _authServiceMock
                .Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(accessToken);

            // Act
            var result = await _controller.LoginAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(accessToken, okResult.Value);

            _authServiceMock.Verify(
                x => x.LoginAsync(dto),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsOkWithConfirmation()
        {
            // Arrange
            var confirmation = new ConfirmationResponseDto
            {
                Message = "User logged out successfully."
            };

            _authServiceMock
                .Setup(x => x.LogoutAsync())
                .ReturnsAsync(confirmation);

            // Act
            var result = await _controller.LogoutAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(confirmation, okResult.Value);

            _authServiceMock.Verify(
                x => x.LogoutAsync(),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsOkWithAccessToken()
        {
            // Arrange
            const string accessToken = "new-access-token";

            _refreshTokenServiceMock
                .Setup(x => x.RefreshTokenAsync())
                .ReturnsAsync(accessToken);

            // Act
            var result = await _controller.RefreshTokenAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(accessToken, okResult.Value);

            _refreshTokenServiceMock.Verify(
                x => x.RefreshTokenAsync(),
                Times.Once);
        }
    }
}