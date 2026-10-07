using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class UserCurrenciesRepositoy(AppDbContext context) : GenericRepository<UserCurrency>(context), IUserCurrenciesRepositoy
    {
        public async Task<IEnumerable<UserCurrency>> GetUserCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.UserCurrencies 
                .Where(uc => uc.UserId == userId && !uc.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<UserCurrency?> GetDefaultCurrencyAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.UserCurrencies
                .Where(uc => uc.UserId == userId && !uc.IsDeleted && uc.IsDefault)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    } 
}
