using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class GoalRepository(AppDbContext context) : GenericRepository<Goals>(context), IGoalRepository
    {
        public async Task<Goals?> GetByPeriodAsync(Guid userId, Guid categoryId, Period period)
        {
            return await context.Goals.Where(g => g.UserId == userId && g.CategoryId == categoryId && g.Period.StartDate == period.StartDate && g.Period.EndDate == period.EndDate && !g.IsDeleted && g.IsActive).FirstOrDefaultAsync();
        }
        public async Task<List<Goals>> GetAllByMonthAsync(Guid userId, Period period)
        {
            return await context.Goals.Where(g => g.UserId == userId && !g.IsDeleted && g.IsActive && g.Period.StartDate == period.StartDate && g.Period.EndDate == period.EndDate).Include(g => g.Category).ToListAsync();
        }
        public async Task<Goals?> GetGoalByIdAsync(Guid id)
        {
            return await context.Goals.Include(g => g.Category).FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
