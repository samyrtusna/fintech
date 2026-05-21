using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Exceptions;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Services
{
    public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        IJwtService jwtService,
        ITokenHasher tokenHasher,
        ICookieService cookieService
            ) : IRefreshTokenService
    {

        public async Task<string> CreateRefreshTokenAsync(Guid userId) 
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var token = jwtService.GenerateRefreshToken();
            var hashedToken = tokenHasher.HashToken(token);
            var expiresAt = DateTime.UtcNow.AddDays(configuration.GetValue<int>("Jwt:RefreshTokenExpiration"));
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = expiresAt,
                IsRevoked = false
            };
            await refreshTokenRepository.AddAsync(refreshToken);
            await refreshTokenRepository.SaveChangesAsync();
            
            return token;
        }

        public async Task<string> RefreshTokenAsync()
        {
            var token = cookieService.RetrieveTokenFromCookie();
            var hashedToken = tokenHasher.HashToken(token);
            var existingToken = await refreshTokenRepository.GetByTokenAsync(hashedToken) ?? throw new UnauthorizedException("Invalid refresh token.");
          
            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                existingToken.IsRevoked = true;
                await refreshTokenRepository.SaveChangesAsync();
                throw new UnauthorizedException("Refresh token has expired.");
            }
            if (existingToken.IsRevoked)
            {
                await RevokeAllTokensAsync(existingToken.UserId);
                throw new UnauthorizedException("Token reuse detected.");
            }

            var newRefreshToken = jwtService.GenerateRefreshToken();
            var newHashedToken = tokenHasher.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = existingToken.UserId,
                Token = newHashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays")),
                IsRevoked = false
            };
            await refreshTokenRepository.AddAsync(newRefreshTokenEntity);
            existingToken.IsRevoked = true;
            existingToken.ReplacedByToken = newHashedToken;
            await refreshTokenRepository.SaveChangesAsync();

            cookieService.SetTokenToCookie(newRefreshToken);
            var user = existingToken.User;
            var accessToken = jwtService.GenerateAccessToken(user);
            return accessToken;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        { 
            ArgumentNullException.ThrowIfNull(token);

            var hashedToken = tokenHasher.HashToken(token);
            var existingToken = await refreshTokenRepository.GetByTokenAsync(hashedToken)?? throw new UnauthorizedException("Invalid refresh token.");

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token has expired.");
            }
            if (existingToken.IsRevoked)
            {
                throw new UnauthorizedException("Token reuse detected, , Token Already revoked!.");
            }

            existingToken.IsRevoked = true;
            refreshTokenRepository.Update(existingToken);
            await refreshTokenRepository.SaveChangesAsync();
            
            cookieService.ClearCookieFromToken();
        }

        public async Task RevokeAllTokensAsync(Guid userId) 
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var tokens = await refreshTokenRepository.GetAllByUserAsync(userId);
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                refreshTokenRepository.Update(token);
            }
            await refreshTokenRepository.SaveChangesAsync();
        }
       
    }
}
