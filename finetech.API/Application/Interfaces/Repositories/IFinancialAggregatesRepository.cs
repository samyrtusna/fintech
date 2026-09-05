using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialAggregatesRepository : IGenericRepository<FinancialAggregates>
    {
        Task<List<FinancialAggregates>> GetFinancialAggregatesByYearAsync(Guid userId, int year, CancellationToken cancellationToken = default);
    }
}
