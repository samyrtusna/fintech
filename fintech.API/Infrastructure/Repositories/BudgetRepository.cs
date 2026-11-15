using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class BudgetRepository(AppDbContext context) : GenericRepository<Budget>(context), IBudgetRepository
    {
        public async Task<Budget?> GetByPeriodAsync(Guid userId,Guid categoryId, Period period)
        {
            return await context.Budgets.Where(b => b.UserId == userId && b.CategoryId == categoryId  && b.Period.StartDate == period.StartDate && b.Period.EndDate == period.EndDate && !b.IsDeleted && b.IsActive).FirstOrDefaultAsync();
        }

        public async Task<List<Budget>> GetAllByMonthAsync(Guid userId, Period period)
        {
            return await context.Budgets.Where(b => b.UserId == userId && !b.IsDeleted && b.IsActive && b.Period.StartDate == period.StartDate && b.Period.EndDate == period.EndDate).Include(b => b.Category).ToListAsync();
        }

        public async Task<Budget?> GetBudgetByIdAsync(Guid id) 
        {
            return await context.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Budget?> GetBudgetByIdAndUserIdAsync(Guid userId, Guid id)
        {
            return await context.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task<Budget?> GetGlobalBudgetByMonthAsync(Guid userId, Period period)
        {
            return await context.Budgets.Where(b => b.UserId == userId && b.Type == BudgetType.Global && !b.IsDeleted && b.IsActive && b.Period.StartDate == period.StartDate && b.Period.EndDate == period.EndDate).FirstOrDefaultAsync();
        }
    }
}
