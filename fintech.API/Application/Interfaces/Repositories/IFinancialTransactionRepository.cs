using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialTransactionRepository : IGenericRepository<FinancialTransaction>
    {
        Task<FinancialTransaction?> GetAsync(Guid id);
        Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(Guid categoryId); 
        Task<IEnumerable<FinancialTransaction>> GetByDayAsync(Guid userId, DateTime day, CancellationToken cancellationToken);
        Task<TopSpendingCategoryDto?> GetTopSpendingCategoryPerDayAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<TopSpendingCategoryDto?> GetTopSpendingCategoryPerMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<TopSpendingCategoryDto?> GetTopSpendingCategoryPerYearAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    }
} 
 