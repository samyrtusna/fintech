using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialAggregatesRepository(AppDbContext context) : GenericRepository<FinancialAggregate>(context), IFinancialAggregatesRepository
    {
        public async Task<List<FinancialAggregate>> GetFinancialAggregatesByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            var startDate = new DateOnly(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await context.FinancialAggregates
                .Where(fa => fa.UserId == userId && fa.Period.StartDate >= startDate && fa.Period.EndDate <= endDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<FinancialAggregate>> GetFinancialAggregatesByNameAsync(Guid userId, string name)
        {
            return await context.FinancialAggregates.Where(fa => fa.UserId == userId && fa.Name == name)
                .AsNoTracking()
                .OrderBy(fa => fa.Period.StartDate)
                .ToListAsync();
        }

        public async Task<List<FinancialAggregate>> GetGlobalAggregatesByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            var startDate = new DateOnly(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await context.FinancialAggregates
                .Where(fa => fa.UserId == userId && fa.PeriodType == AggregateType.GlobalAggregate && fa.Period.StartDate >= startDate && fa.Period.EndDate <= endDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
