using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialAggregatesRepository(AppDbContext context) : GenericRepository<FinancialAggregates>(context), IFinancialAggregatesRepository
    {
        public async Task<List<FinancialAggregates>> GetFinancialAggregatesByYearAsync(Guid userId, int year, CancellationToken cancellationToken = default)
        {
            return await context.FinancialAggregates
                .Where(fa => fa.UserId == userId && fa.Year == year)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
