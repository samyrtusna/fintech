using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Services;
using fintech.API.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace fintech.Tests.Application.Services
{
    public class RefreshTokenServiceTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ITokenHasher> _tokenHasherMock;
        private readonly Mock<ICookieService> _cookieServiceMock;

        private readonly IConfiguration _configuration;

        private readonly RefreshTokenService _service;

        public RefreshTokenServiceTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _tokenHasherMock = new Mock<ITokenHasher>();
            _cookieServiceMock = new Mock<ICookieService>();

            var configurationValues = new Dictionary<string, string?>
            {
                ["Jwt:RefreshTokenExpiration"] = "7"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            _service = new RefreshTokenService(
                _refreshTokenRepositoryMock.Object,
                _configuration,
                _jwtServiceMock.Object,
                _tokenHasherMock.Object,
                _cookieServiceMock.Object);
        }

        [Fact]
        public async Task CreateRefreshTokenAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateRefreshTokenAsync(Guid.Empty));
        }

        [Fact]
        public async Task CreateRefreshTokenAsync_ValidUserId_GeneratesAndReturnsToken()
        {
            // Arrange
            var userId = Guid.NewGuid();

            const string refreshToken = "generated-refresh-token";
            const string hashedToken = "hashed-refresh-token";

            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(refreshToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(refreshToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateRefreshTokenAsync(userId);

            // Assert
            Assert.Equal(refreshToken, result);

            _jwtServiceMock.Verify(
                j => j.GenerateRefreshToken(),
                Times.Once);

            _tokenHasherMock.Verify(
                h => h.HashToken(refreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<RefreshToken>(token =>
                        token.UserId == userId &&
                        token.Token == hashedToken &&
                        token.IsRevoked == false),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateRefreshTokenAsync_ValidUserId_CreatesTokenWithExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();

            const string refreshToken = "generated-refresh-token";
            const string hashedToken = "hashed-refresh-token";

            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(refreshToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(refreshToken))
                .Returns(hashedToken);

            RefreshToken? createdToken = null;

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()))
                .Callback<RefreshToken, CancellationToken>((token, _) =>
                {
                    createdToken = token;
                })
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var before = DateTime.UtcNow;

            await _service.CreateRefreshTokenAsync(userId);

            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(createdToken);
            Assert.Equal(userId, createdToken!.UserId);
            Assert.Equal(hashedToken, createdToken.Token);
            Assert.False(createdToken.IsRevoked);

            Assert.True(
                createdToken.ExpiresAt >= before.AddDays(7) &&
                createdToken.ExpiresAt <= after.AddDays(7));
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ThrowsNotFoundException()
        {
            // Arrange
            const string rawToken = "invalid-token";
            const string hashedToken = "hashed-invalid-token";

            _cookieServiceMock
                .Setup(c => c.RetrieveTokenFromCookie())
                .Returns(rawToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(hashedToken))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.RefreshTokenAsync());

            _cookieServiceMock.Verify(
                c => c.RetrieveTokenFromCookie(),
                Times.Once);

            _tokenHasherMock.Verify(
                h => h.HashToken(rawToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.GetByTokenAsync(hashedToken),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ExpiredToken_RevokesTokenAndThrowsUnauthorizedException()
        {
            // Arrange
            const string rawToken = "expired-token";
            const string hashedToken = "hashed-expired-token";

            var userId = Guid.NewGuid();

            var existingToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-10),
                IsRevoked = false
            };

            _cookieServiceMock
                .Setup(c => c.RetrieveTokenFromCookie())
                .Returns(rawToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(hashedToken))
                .ReturnsAsync(existingToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.RefreshTokenAsync());

            Assert.True(existingToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.Update(It.IsAny<RefreshToken>()),
                Times.Never);

            _jwtServiceMock.Verify(
                j => j.GenerateRefreshToken(),
                Times.Never);

            _cookieServiceMock.Verify(
                c => c.SetTokenToCookie(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_RevokedToken_RevokesAllUserTokensAndThrowsUnauthorizedException()
        {
            // Arrange
            const string rawToken = "revoked-token";
            const string hashedToken = "hashed-revoked-token";

            var userId = Guid.NewGuid();

            var existingToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                IsRevoked = true
            };

            _cookieServiceMock
                .Setup(c => c.RetrieveTokenFromCookie())
                .Returns(rawToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(hashedToken))
                .ReturnsAsync(existingToken);

            var tokens = new List<RefreshToken>
    {
        existingToken,
        new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "another-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(5),
            IsRevoked = false
        }
    };

            _refreshTokenRepositoryMock
                .Setup(r => r.GetAllByUserAsync(userId))
                .ReturnsAsync(tokens);

            _refreshTokenRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.RefreshTokenAsync());

            Assert.All(tokens, token =>
                Assert.True(token.IsRevoked));

            _refreshTokenRepositoryMock.Verify(
                r => r.GetAllByUserAsync(userId),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.Update(It.IsAny<RefreshToken>()),
                Times.Exactly(tokens.Count));

            _refreshTokenRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _jwtServiceMock.Verify(
                j => j.GenerateRefreshToken(),
                Times.Never);

            _cookieServiceMock.Verify(
                c => c.SetTokenToCookie(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_RotatesTokenAndReturnsAccessToken()
        {
            // Arrange
            const string rawToken = "old-refresh-token";
            const string hashedOldToken = "hashed-old-token";

            const string newRefreshToken = "new-refresh-token";
            const string hashedNewToken = "hashed-new-token";

            const string accessToken = "new-access-token";

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "dummy-password-hash",
                BaseCurrency = "USD"
            };

            var existingToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = hashedOldToken,
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                IsRevoked = false,
                User = user
            };

            _cookieServiceMock
                .Setup(c => c.RetrieveTokenFromCookie())
                .Returns(rawToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(rawToken))
                .Returns(hashedOldToken);

            _refreshTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(hashedOldToken))
                .ReturnsAsync(existingToken);

            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(newRefreshToken);

            _tokenHasherMock
                .Setup(h => h.HashToken(newRefreshToken))
                .Returns(hashedNewToken);

            _jwtServiceMock
                .Setup(j => j.GenerateAccessToken(user))
                .Returns(accessToken);

            RefreshToken? newTokenEntity = null;

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()))
                .Callback<RefreshToken, CancellationToken>((token, _) =>
                {
                    newTokenEntity = token;
                })
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RefreshTokenAsync();

            // Assert
            Assert.Equal(accessToken, result);

            // Old token was revoked
            Assert.True(existingToken.IsRevoked);

            // Old token points to replacement
            Assert.Equal(hashedNewToken, existingToken.ReplacedByToken);

            // New refresh token entity
            Assert.NotNull(newTokenEntity);
            Assert.Equal(userId, newTokenEntity!.UserId);
            Assert.Equal(hashedNewToken, newTokenEntity.Token);
            Assert.False(newTokenEntity.IsRevoked);

            Assert.InRange(
                newTokenEntity!.ExpiresAt,
                DateTime.UtcNow.AddDays(7).AddSeconds(-5),
                DateTime.UtcNow.AddDays(7).AddSeconds(5));

            // New cookie was created
            _cookieServiceMock.Verify(
                c => c.SetTokenToCookie(newRefreshToken),
                Times.Once);

            // New access token was generated
            _jwtServiceMock.Verify(
                j => j.GenerateAccessToken(user),
                Times.Once);

            // Persistence
            _refreshTokenRepositoryMock.Verify(
                r => r.AddAsync(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_NullToken_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RevokeRefreshTokenAsync(null!));
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_InvalidToken_ThrowsUnauthorizedException()
        {
            var rawToken = "invalid-token";
            var hashedToken = "hashed-token";

            _tokenHasherMock
                .Setup(x => x.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(hashedToken))
                .ReturnsAsync((RefreshToken?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _service.RevokeRefreshTokenAsync(rawToken));

            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(hashedToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _cookieServiceMock.Verify(
                x => x.ClearCookieFromToken(),
                Times.Never);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ExpiredToken_ThrowsUnauthorizedException()
        {
            var rawToken = "expired-token";
            var hashedToken = "hashed-token";

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
                IsRevoked = false
            };

            _tokenHasherMock
                .Setup(x => x.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(hashedToken))
                .ReturnsAsync(refreshToken);

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _service.RevokeRefreshTokenAsync(rawToken));

            Assert.False(refreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _cookieServiceMock.Verify(
                x => x.ClearCookieFromToken(),
                Times.Never);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_AlreadyRevokedToken_ThrowsUnauthorizedException()
        {
            var rawToken = "revoked-token";
            var hashedToken = "hashed-token";

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true
            };

            _tokenHasherMock
                .Setup(x => x.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(hashedToken))
                .ReturnsAsync(refreshToken);

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _service.RevokeRefreshTokenAsync(rawToken));

            _refreshTokenRepositoryMock.Verify(
                x => x.Update(It.IsAny<RefreshToken>()),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _cookieServiceMock.Verify(
                x => x.ClearCookieFromToken(),
                Times.Never);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ValidToken_RevokesTokenAndClearsCookie()
        {
            var rawToken = "valid-token";
            var hashedToken = "hashed-token";

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _tokenHasherMock
                .Setup(x => x.HashToken(rawToken))
                .Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(hashedToken))
                .ReturnsAsync(refreshToken);

            await _service.RevokeRefreshTokenAsync(rawToken);

            Assert.True(refreshToken.IsRevoked);

            _refreshTokenRepositoryMock.Verify(
                x => x.Update(refreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _cookieServiceMock.Verify(
                x => x.ClearCookieFromToken(),
                Times.Once);
        }

        [Fact]
        public async Task RevokeAllTokensAsync_EmptyUserId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.RevokeAllTokensAsync(Guid.Empty));

            _refreshTokenRepositoryMock.Verify(
                x => x.GetAllByUserAsync(It.IsAny<Guid>()),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RevokeAllTokensAsync_ValidUser_RevokesAllTokens()
        {
            var userId = Guid.NewGuid();

            var tokens = new List<RefreshToken>
    {
        new RefreshToken
        {
            UserId = userId,
            Token = "token-1",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        },
        new RefreshToken
        {
            UserId = userId,
            Token = "token-2",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        },
        new RefreshToken
        {
            UserId = userId,
            Token = "token-3",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = true
        }
    };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetAllByUserAsync(userId))
                .ReturnsAsync(tokens);

            await _service.RevokeAllTokensAsync(userId);

            Assert.All(tokens, token => Assert.True(token.IsRevoked));

            _refreshTokenRepositoryMock.Verify(
                x => x.GetAllByUserAsync(userId),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.Update(It.IsAny<RefreshToken>()),
                Times.Exactly(3));

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RevokeAllTokensAsync_UserHasNoTokens_SavesChanges()
        {
            var userId = Guid.NewGuid();

            _refreshTokenRepositoryMock
                .Setup(x => x.GetAllByUserAsync(userId))
                .ReturnsAsync([]);

            await _service.RevokeAllTokensAsync(userId);

            _refreshTokenRepositoryMock.Verify(
                x => x.GetAllByUserAsync(userId),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.Update(It.IsAny<RefreshToken>()),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}