using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialIntelligenceSnapshotRepository : IGenericRepository<FinancialIntelligenceSnapshot>
    {
        Task<FinancialIntelligenceSnapshot?> GetByDayAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshot?> GetLatestDailyAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshot?>GetByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialIntelligenceSnapshot>> GetMonthlyAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshot?> GetByYearAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialIntelligenceSnapshot>> GetYearlyAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    }
}  
