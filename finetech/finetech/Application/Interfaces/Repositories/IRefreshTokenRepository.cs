using fintech.Domain.Entities;

namespace fintech.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
 