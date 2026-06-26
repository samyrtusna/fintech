using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialIndicatorRepository : IGenericRepository<FinancialIndicator>
    {
        Task AddRangeAsync(IEnumerable<FinancialIndicator> indicators, CancellationToken cancellationToken = default);
    }
}
