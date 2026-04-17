using fintech.Application.Exceptions;
using fintech.Application.Interfaces.Services;

namespace fintech.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public CookieService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }
        public void SetTokenToCookie(string token)
        {
            ArgumentNullException.ThrowIfNull(token); 
            var httpContext = _contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"))
            };
            httpContext.Response.Cookies.Append("refreshToken", token, refreshTokenCookieOptions);
        }

        public string RetrieveTokenFromCookie()
        {
            var httpContext = _contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            var token = httpContext.Request.Cookies["refreshToken"] ?? throw new UnauthorizedException("Refresh token is missing.");

            return token;
        }

        public void ClearTokenCookie()
        {
            var httpContext = _contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
