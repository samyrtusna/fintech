using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class BudgetRepository(AppDbContext context) : GenericRepository<Budget>(context), IBudgetRepository
    {
        public async Task<Budget?> GetByPeriodAsync(Guid userId, Period period)
        {
            return await context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && !b.IsDeleted && b.IsActive && b.Period == period);
        }

        public Task<List<Budget>>GetAllByMonthAsync(Guid userId, Period period)
        {
            return context.Budgets.Where(b => b.UserId == userId && !b.IsDeleted && b.IsActive && b.Period == period).ToListAsync();
        }

        public async Task<Budget?> GetBudgetByIdAsync(Guid id) 
        {
            return await context.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
