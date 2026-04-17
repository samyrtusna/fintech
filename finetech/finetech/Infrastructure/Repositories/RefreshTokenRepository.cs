using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace fintech.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId)
            {
                return await _context.RefreshTokens.Include(rt => rt.User)
                .Where(rt => rt.UserId == userId).ToListAsync();
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
            {
                return await _context.RefreshTokens.Include(rt => rt.User) 
                .FirstOrDefaultAsync(rt => rt.Token == token);
            }
    }
}
