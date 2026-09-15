using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace fintech.Tests.Infrastructure.Services
{
    public class JwtServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "this-is-a-test-secret-key-with-at-least-32-bytes",
                ["Jwt:Issuer"] = "fintech-api",
                ["Jwt:Audience"] = "fintech-client",
                ["Jwt:AccessTokenExpiration"] = "60"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _jwtService = new JwtService(_configuration);
        }

        private static User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashed-password",
                Role = UserRole.User,
                BaseCurrency = "USD",
                CreatedAt = DateTime.UtcNow
            };
        }

        // =========================================================
        // GenerateAccessToken
        // =========================================================

        [Fact]
        public void GenerateAccessToken_ValidUser_ReturnsNonEmptyToken()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var token = _jwtService.GenerateAccessToken(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ReturnsValidJwtWithThreeSections()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var token = _jwtService.GenerateAccessToken(user);

            // Assert
            var sections = token.Split('.');

            Assert.Equal(3, sections.Length);
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ContainsExpectedClaims()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            Assert.Equal(
                user.Id.ToString(),
                token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            Assert.Equal(
                user.Username,
                token.Claims.First(c => c.Type == ClaimTypes.Name).Value);

            Assert.Equal(
                user.Email,
                token.Claims.First(c => c.Type == ClaimTypes.Email).Value);

            Assert.Equal(
                user.Role.ToString(),
                token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ContainsBaseCurrencyClaim()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            var userDataClaims = token.Claims
                .Where(c => c.Type == ClaimTypes.UserData)
                .Select(c => c.Value)
                .ToList();

            Assert.Contains(user.BaseCurrency, userDataClaims);
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ContainsCreatedAtClaim()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            var userDataClaims = token.Claims
                .Where(c => c.Type == ClaimTypes.UserData)
                .Select(c => c.Value)
                .ToList();

            Assert.Contains(user.CreatedAt.ToString("o"), userDataClaims);
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ContainsJtiClaim()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            var jti = token.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            Assert.NotNull(jti);
            Assert.True(Guid.TryParse(jti.Value, out _));
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_ContainsExpectedIssuerAndAudience()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            Assert.Equal("fintech-api", token.Issuer);
            Assert.Equal("fintech-client", token.Audiences.Single());
        }

        [Fact]
        public void GenerateAccessToken_ValidUser_HasExpectedExpiration()
        {
            // Arrange
            var user = CreateUser();
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var tokenString = _jwtService.GenerateAccessToken(user);

            var afterGeneration = DateTime.UtcNow;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert

            var expectedMinimum = beforeGeneration.AddMinutes(60);
            var expectedMaximum = afterGeneration.AddMinutes(60);

            Assert.InRange(
                token.ValidTo,
                expectedMinimum.AddSeconds(-1),
                expectedMaximum.AddSeconds(1));
        }

        [Fact]
        public void GenerateAccessToken_NullUser_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => _jwtService.GenerateAccessToken(null!));
        }

        [Fact]
        public void GenerateAccessToken_SameUserTwice_GeneratesDifferentJti()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var token1 = _jwtService.GenerateAccessToken(user);
            var token2 = _jwtService.GenerateAccessToken(user);

            var handler = new JwtSecurityTokenHandler();

            var jwt1 = handler.ReadJwtToken(token1);
            var jwt2 = handler.ReadJwtToken(token2);

            var jti1 = jwt1.Claims
                .First(c => c.Type == JwtRegisteredClaimNames.Jti)
                .Value;

            var jti2 = jwt2.Claims
                .First(c => c.Type == JwtRegisteredClaimNames.Jti)
                .Value;

            // Assert
            Assert.NotEqual(jti1, jti2);
        }

        // =========================================================
        // GenerateRefreshToken
        // =========================================================

        [Fact]
        public void GenerateRefreshToken_ReturnsNonEmptyToken()
        {
            // Act
            var token = _jwtService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsValidBase64()
        {
            // Act
            var token = _jwtService.GenerateRefreshToken();

            // Assert
            var exception = Record.Exception(
                () => Convert.FromBase64String(token));

            Assert.Null(exception);
        }

        [Fact]
        public void GenerateRefreshToken_Returns32Bytes()
        {
            // Act
            var token = _jwtService.GenerateRefreshToken();

            // Assert
            var decoded = Convert.FromBase64String(token);

            Assert.Equal(32, decoded.Length);
        }

        [Fact]
        public void GenerateRefreshToken_GeneratedTwice_ReturnsDifferentTokens()
        {
            // Act
            var token1 = _jwtService.GenerateRefreshToken();
            var token2 = _jwtService.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }
    }
}