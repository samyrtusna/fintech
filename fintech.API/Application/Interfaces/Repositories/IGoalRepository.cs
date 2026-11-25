using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IGoalRepository: IGenericRepository<Goals>
    {
        Task<Goals?> GetByPeriodAsync(Guid userId, Guid categoryId, Period period);
        Task<List<Goals>> GetAllByMonthAsync(Guid userId, Period period);
        Task<Goals?> GetGoalByIdAsync(Guid id);
    }
}
