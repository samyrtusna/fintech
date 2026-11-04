using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        Task<Budget?> GetByPeriodAsync(Guid userId, Period period);
        Task<List<Budget>> GetAllByMonthAsync(Guid userId, Period period);
        Task<Budget?> GetBudgetByIdAsync(Guid id);
    }
}
