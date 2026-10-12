using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialTransactionRepository : IGenericRepository<FinancialTransaction>
    {
        Task<FinancialTransaction?> GetAsync(Guid id);
        Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(Guid categoryId); 
        Task<IEnumerable<FinancialTransaction>> GetByDayAsync(Guid userId, DateTime day, CancellationToken cancellationToken);
        Task<List<FinancialTransaction>> GetByTypeByMonth(Guid userId, DateTime date, FinancialType type, CancellationToken cancellationToken = default);
        Task<List<FinancialTransaction>> GetByTypeByYear(Guid userId, DateTime date, FinancialType type, CancellationToken cancellationToken = default); 
    } 
} 
  