using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Exceptions;

namespace fintech.API.Infrastructure.Services
{
    public class CookieService(IConfiguration configuration, IHttpContextAccessor contextAccessor) : ICookieService
    {

        public void SetTokenToCookie(string token)
        {
            ArgumentNullException.ThrowIfNull(token); 
            var httpContext = contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path= "/",
                Expires = DateTime.UtcNow.AddDays(configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"))
            };
            httpContext.Response.Cookies.Append("refreshToken", token, refreshTokenCookieOptions);
        }

        public string RetrieveTokenFromCookie()
        {
            var httpContext = contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            var token = httpContext.Request.Cookies["refreshToken"] ?? throw new UnauthorizedException("Refresh token is missing.");

            return token;
        }

        public void ClearCookieFromToken() 
        {
            var httpContext = contextAccessor.HttpContext ?? throw new UnauthorizedException("No HTTP context available.");
            httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
