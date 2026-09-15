using fintech.API.Application.Exceptions;
using fintech.API.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace fintech.Tests.Infrastructure.Services
{
    public class CookieServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly CookieService _cookieService;

        public CookieServiceTests()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:RefreshTokenExpiration"] = "7"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _httpContext = new DefaultHttpContext();

            _contextAccessor = new Mock<IHttpContextAccessor>();
            _contextAccessor
                .Setup(x => x.HttpContext)
                .Returns(_httpContext);

            _cookieService = new CookieService(
                _configuration,
                _contextAccessor.Object);
        }

        // =========================================================
        // SetTokenToCookie
        // =========================================================

        [Fact]
        public void SetTokenToCookie_ValidToken_AddsRefreshTokenCookie()
        {
            // Arrange
            var token = "test-refresh-token";

            // Act
            _cookieService.SetTokenToCookie(token);

            // Assert
            var setCookieHeader =
                _httpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("refreshToken=test-refresh-token", setCookieHeader);
        }

        [Fact]
        public void SetTokenToCookie_ValidToken_SetsHttpOnlyCookie()
        {
            // Arrange
            var token = "test-refresh-token";

            // Act
            _cookieService.SetTokenToCookie(token);

            // Assert
            var setCookieHeader =
                _httpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("httponly", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SetTokenToCookie_ValidToken_SetsSecureCookie()
        {
            // Arrange
            var token = "test-refresh-token";

            // Act
            _cookieService.SetTokenToCookie(token);

            // Assert
            var setCookieHeader =
                _httpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("secure", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SetTokenToCookie_ValidToken_SetsSameSiteNone()
        {
            // Arrange
            var token = "test-refresh-token";

            // Act
            _cookieService.SetTokenToCookie(token);

            // Assert
            var setCookieHeader =
                _httpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("samesite=none", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SetTokenToCookie_NullToken_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => _cookieService.SetTokenToCookie(null!));
        }

        [Fact]
        public void SetTokenToCookie_NoHttpContext_ThrowsUnauthorizedException()
        {
            // Arrange
            _contextAccessor
                .Setup(x => x.HttpContext)
                .Returns((HttpContext?)null);

            // Act & Assert
            var exception = Assert.Throws<UnauthorizedException>(
                () => _cookieService.SetTokenToCookie("test-token"));

            Assert.Equal(
                "No HTTP context available.",
                exception.Message);
        }

        // =========================================================
        // RetrieveTokenFromCookie
        // =========================================================

        [Fact]
        public void RetrieveTokenFromCookie_ExistingCookie_ReturnsToken()
        {
            // Arrange
            _httpContext.Request.Headers["Cookie"] =
                "refreshToken=test-refresh-token";

            // Act
            var result = _cookieService.RetrieveTokenFromCookie();

            // Assert
            Assert.Equal("test-refresh-token", result);
        }

        [Fact]
        public void RetrieveTokenFromCookie_MissingCookie_ThrowsUnauthorizedException()
        {
            // Act & Assert
            var exception = Assert.Throws<UnauthorizedException>(
                () => _cookieService.RetrieveTokenFromCookie());

            Assert.Equal(
                "Refresh token is missing.",
                exception.Message);
        }

        [Fact]
        public void RetrieveTokenFromCookie_NoHttpContext_ThrowsUnauthorizedException()
        {
            // Arrange
            _contextAccessor
                .Setup(x => x.HttpContext)
                .Returns((HttpContext?)null);

            // Act & Assert
            var exception = Assert.Throws<UnauthorizedException>(
                () => _cookieService.RetrieveTokenFromCookie());

            Assert.Equal(
                "No HTTP context available.",
                exception.Message);
        }

        // =========================================================
        // ClearCookieFromToken
        // =========================================================

        [Fact]
        public void ClearCookieFromToken_AddsDeletionCookie()
        {
            // Act
            _cookieService.ClearCookieFromToken();

            // Assert
            var setCookieHeader =
                _httpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("refreshToken=", setCookieHeader);
            Assert.Contains("expires=", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ClearCookieFromToken_NoHttpContext_ThrowsUnauthorizedException()
        {
            // Arrange
            _contextAccessor
                .Setup(x => x.HttpContext)
                .Returns((HttpContext?)null);

            // Act & Assert
            var exception = Assert.Throws<UnauthorizedException>(
                () => _cookieService.ClearCookieFromToken());

            Assert.Equal(
                "No HTTP context available.",
                exception.Message);
        }
    }
}