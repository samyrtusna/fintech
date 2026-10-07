using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialAggregatesRepository : IGenericRepository<FinancialAggregate>
    {
        Task<List<FinancialAggregate>> GetFinancialAggregatesByYearAsync(Guid userId, int year, CancellationToken cancellationToken = default);
    }
}
