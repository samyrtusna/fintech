namespace fintech.API.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(Guid userId);
        Task<string> RefreshTokenAsync();
        Task RevokeRefreshTokenAsync(string token); 
        Task RevokeAllTokensAsync(Guid userId); 
    }
}
