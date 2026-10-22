using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialTransactionRepository : IGenericRepository<FinancialTransaction>
    {
        Task<FinancialTransaction?> GetAsync(Guid id);
        Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(Guid categoryId); 
        Task<IEnumerable<FinancialTransaction>> GetByDayAsync(Guid userId, DateTime day, CancellationToken cancellationToken);
        Task<List<FinancialTransaction>> GetByMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<List<FinancialTransaction>> GetByYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<List<AggregateByPeriodDto>> GetByTypeByMonth(Guid userId, FinancialType type, CancellationToken cancellationToken = default);
        Task<List<AggregateByPeriodDto>> GetByCategoryByMonth(Guid userId, string categoryName, CancellationToken cancellationToken = default);
    }  
} 
    