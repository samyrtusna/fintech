using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        Task<Budget?> GetByPeriodAsync(Guid userId, Guid categoryId, Period period);
        Task<List<Budget>> GetAllByMonthAsync(Guid userId, DateTime date);
        Task<Budget?> GetBudgetByIdAsync(Guid id);
        Task<Budget?> GetGlobalBudgetByMonthAsync(Guid userId, Period period);
    }
}
