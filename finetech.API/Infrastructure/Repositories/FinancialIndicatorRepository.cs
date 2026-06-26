using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialIndicatorRepository(AppDbContext context) : GenericRepository<FinancialIndicator>(context), IFinancialIndicatorRepository
    {
        public async Task AddRangeAsync(IEnumerable<FinancialIndicator> indicators, CancellationToken cancellationToken = default)
        {
            await context.FinancialIndicators.AddRangeAsync(indicators, cancellationToken);
        }
    }
}
