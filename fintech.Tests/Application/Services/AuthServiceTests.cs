using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Services;
using fintech.API.Application.Exceptions;
using fintech.API.Domain.Entities;
using Moq;

namespace fintech.Tests.Application.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ICookieService> _cookieServiceMock;

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _cookieServiceMock = new Mock<ICookieService>();

            _service = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                _refreshTokenServiceMock.Object,
                _passwordHasherMock.Object,
                _cookieServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_NullDto_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RegisterAsync(null!));

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(It.IsAny<string>()),
                Times.Never);

            _userRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsDuplicateValueException()
        {
            var dto = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = dto.Email,
                PasswordHash = "existing-password-hash",
                BaseCurrency = "USD"
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<DuplicateValueException>(
                () => _service.RegisterAsync(dto));

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(dto.Email),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _userRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _passwordHasherMock.Verify(
                x => x.HashPassword(It.IsAny<string>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ValidDto_CreatesUserAndReturnsAccessToken()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                Username = "newuser"
            };

            const string passwordHash = "hashed-password";
            const string accessToken = "access-token";
            const string refreshToken = "refresh-token";

            User? createdUser = null;

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            _passwordHasherMock
                .Setup(x => x.HashPassword(dto.Password))
                .Returns(passwordHash);

            _userRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, _) =>
                {
                    createdUser = user;
                })
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                .Returns(accessToken);

            _refreshTokenServiceMock
                .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.Equal(accessToken, result);

            // User was created
            Assert.NotNull(createdUser);
            Assert.Equal(dto.Email, createdUser!.Email);
            Assert.Equal(dto.Username, createdUser.Username);
            Assert.Equal(passwordHash, createdUser.PasswordHash);

            // Password was hashed
            _passwordHasherMock.Verify(
                x => x.HashPassword(dto.Password),
                Times.Once);

            // User was persisted
            _userRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            // Tokens
            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(createdUser),
                Times.Once);

            _refreshTokenServiceMock.Verify(
                x => x.CreateRefreshTokenAsync(createdUser.Id),
                Times.Once);

            // Refresh token cookie
            _cookieServiceMock.Verify(
                x => x.SetTokenToCookie(refreshToken),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_NullDto_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.LoginAsync(null!));

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(It.IsAny<string>()),
                Times.Never);

            _passwordHasherMock.Verify(
                x => x.VerifyPassword(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_UserDoesNotExist_ThrowsBadRequestException()
        {
            var dto = new LoginRequestDto
            {
                Email = "unknown@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.LoginAsync(dto));

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(dto.Email),
                Times.Once);

            _passwordHasherMock.Verify(
                x => x.VerifyPassword(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _refreshTokenServiceMock.Verify(
                x => x.CreateRefreshTokenAsync(It.IsAny<Guid>()),
                Times.Never);

            _cookieServiceMock.Verify(
                x => x.SetTokenToCookie(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = dto.Email,
                PasswordHash = "correct-password-hash",
                BaseCurrency = "USD"
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(x => x.VerifyPassword(
                    dto.Password,
                    user.PasswordHash))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.LoginAsync(dto));

            // User lookup
            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(dto.Email),
                Times.Once);

            // Password verification
            _passwordHasherMock.Verify(
                x => x.VerifyPassword(
                    dto.Password,
                    user.PasswordHash),
                Times.Once);

            // No authentication tokens should be created
            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _refreshTokenServiceMock.Verify(
                x => x.CreateRefreshTokenAsync(It.IsAny<Guid>()),
                Times.Never);

            _cookieServiceMock.Verify(
                x => x.SetTokenToCookie(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAccessTokenAndSetsRefreshTokenCookie()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "CorrectPassword123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = dto.Email,
                PasswordHash = "correct-password-hash",
                BaseCurrency = "USD"
            };

            const string accessToken = "access-token";
            const string refreshToken = "refresh-token";

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(x => x.VerifyPassword(
                    dto.Password,
                    user.PasswordHash))
                .Returns(true);

            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(user))
                .Returns(accessToken);

            _refreshTokenServiceMock
                .Setup(x => x.CreateRefreshTokenAsync(user.Id))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.Equal(accessToken, result);

            // User lookup
            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(dto.Email),
                Times.Once);

            // Password verification
            _passwordHasherMock.Verify(
                x => x.VerifyPassword(
                    dto.Password,
                    user.PasswordHash),
                Times.Once);

            // Access token
            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(user),
                Times.Once);

            // Refresh token
            _refreshTokenServiceMock.Verify(
                x => x.CreateRefreshTokenAsync(user.Id),
                Times.Once);

            // Refresh token cookie
            _cookieServiceMock.Verify(
                x => x.SetTokenToCookie(refreshToken),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ValidToken_RevokesRefreshTokenAndReturnsConfirmation()
        {
            // Arrange
            const string refreshToken = "refresh-token";

            _cookieServiceMock
                .Setup(x => x.RetrieveTokenFromCookie())
                .Returns(refreshToken);

            // Act
            var result = await _service.LogoutAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                " User Logged out successful.",
                result.Message);

            _cookieServiceMock.Verify(
                x => x.RetrieveTokenFromCookie(),
                Times.Once);

            _refreshTokenServiceMock.Verify(
                x => x.RevokeRefreshTokenAsync(refreshToken),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_InvalidToken_PropagatesUnauthorizedException()
        {
            // Arrange
            const string refreshToken = "invalid-refresh-token";

            _cookieServiceMock
                .Setup(x => x.RetrieveTokenFromCookie())
                .Returns(refreshToken);

            _refreshTokenServiceMock
                .Setup(x => x.RevokeRefreshTokenAsync(refreshToken))
                .ThrowsAsync(
                    new UnauthorizedException("Invalid refresh token."));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _service.LogoutAsync());

            _cookieServiceMock.Verify(
                x => x.RetrieveTokenFromCookie(),
                Times.Once);

            _refreshTokenServiceMock.Verify(
                x => x.RevokeRefreshTokenAsync(refreshToken),
                Times.Once);
        }
    }
}