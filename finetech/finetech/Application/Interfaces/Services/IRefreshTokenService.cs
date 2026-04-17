using fintech.Domain.Entities;

namespace fintech.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<string> AddNewRefreshToken(Guid userId);
        Task<string> RefreshAsync();
        Task RevokeToken(string token);
        Task RevokeAllTokensForUser(Guid userId);
    }
}
