using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialAggregatesRepository : IGenericRepository<FinancialAggregate>
    {
        Task<List<FinancialAggregate>> GetFinancialAggregatesByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<List<FinancialAggregate>> GetFinancialAggregatesByNameAsync(Guid userId, string name);
        Task<List<FinancialAggregate>> GetGlobalAggregatesByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    }
}
