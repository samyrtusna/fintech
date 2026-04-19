using fintech.Application.Exceptions;
using fintech.Application.Interfaces.Repositories;
using fintech.Application.Interfaces.Services;
using fintech.Domain.Entities;

namespace fintech.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly ITokenHasher _tokenHasher;
        private readonly ICookieService _cookieService;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, 
            IConfiguration configuration, 
            IJwtService jwtService, 
            ITokenHasher tokenHasher, 
            ICookieService cookieService
            )
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _jwtService = jwtService;
            _tokenHasher = tokenHasher;
            _cookieService = cookieService;
        }

        public async Task<string> AddNewRefreshToken(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var token = _jwtService.GenerateRefreshToken();
            var hashedToken = _tokenHasher.HashToken(token);
            var expiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpiration"));
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = expiresAt,
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
            
            return token;
        }

        public async Task<string> RefreshAsync()
        {
            var token = _cookieService.RetrieveTokenFromCookie();
            var hashedToken = _tokenHasher.HashToken(token);
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(hashedToken) ?? throw new UnauthorizedException("Invalid refresh token.");
          
            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                existingToken.IsRevoked = true;
                await _refreshTokenRepository.SaveChangesAsync();
                throw new UnauthorizedException("Refresh token has expired.");
            }
            if (existingToken.IsRevoked)
            {
                await RevokeAllTokensForUser(existingToken.UserId);
                throw new UnauthorizedException("Token reuse detected.");
            }

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var newHashedToken = _tokenHasher.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = existingToken.UserId,
                Token = newHashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays")),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
            existingToken.IsRevoked = true;
            existingToken.ReplacedByToken = newHashedToken;
            await _refreshTokenRepository.SaveChangesAsync();

            _cookieService.SetTokenToCookie(newRefreshToken);
            var user = existingToken.User;
            var accessToken = _jwtService.GenerateAccessToken(user);
            return accessToken;
        }

        public async Task RevokeToken(string token)
        {
            ArgumentNullException.ThrowIfNull(token);

            var hashedToken = _tokenHasher.HashToken(token);
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(hashedToken)?? throw new UnauthorizedException("Invalid refresh token.");

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token has expired.");
            }
            if (existingToken.IsRevoked)
            {
                throw new UnauthorizedException("Token reuse detected, , Token Already revoked!.");
            }

            existingToken.IsRevoked = true;
            _refreshTokenRepository.Update(existingToken);
            await _refreshTokenRepository.SaveChangesAsync();
            
            _cookieService.ClearTokenCookie();
        }

        public async Task RevokeAllTokensForUser(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var tokens = await _refreshTokenRepository.GetAllByUserIdAsync(userId);
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                _refreshTokenRepository.Update(token);
            }
            await _refreshTokenRepository.SaveChangesAsync();
        }
       
    }
}
