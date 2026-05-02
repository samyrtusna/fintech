using fintech.Domain.Entities;

namespace fintech.Application.Interfaces.Repositories
{
    public interface ITransactionRepository : IGenericRepository<FinancialTransaction>
    {
        Task<FinancialTransaction?> GetAsync(Guid id);
        Task<IEnumerable<FinancialTransaction>> GetByCategoryIdAsync(Guid categoryId);
    }
}
