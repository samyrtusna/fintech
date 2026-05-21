using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAsync(string token); 
    }
}
 