using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;


namespace fintech.API.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : GenericRepository<RefreshToken>(context), IRefreshTokenRepository
    {

        public async Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId)
            {
                return await context.RefreshTokens.Include(rt => rt.User) 
                .Where(rt => rt.UserId == userId).ToListAsync();
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
            {
                return await context.RefreshTokens.Include(rt => rt.User) 
                .FirstOrDefaultAsync(rt => rt.Token == token);
            }
    }
}
