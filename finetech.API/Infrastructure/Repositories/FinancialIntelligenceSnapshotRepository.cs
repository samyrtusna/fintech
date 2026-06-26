using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialIntelligenceSnapshotRepository(AppDbContext context) : GenericRepository<FinancialIntelligenceSnapshot>(context), IFinancialIntelligenceSnapshotRepository
    {
        public async Task<FinancialIntelligenceSnapshot?> GetByDayAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialIntelligenceSnapshots 
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId &&  
                s.PeriodStart.Year == date.Year && 
                s.PeriodStart.Month == date.Month && 
                s.PeriodStart.Day == date.Day && 
                s.PeriodType == SnapshotPeriodType.Daily, cancellationToken);
        }

        public async Task<FinancialIntelligenceSnapshot?> GetLatestDailyAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.FinancialIntelligenceSnapshots
                .Where(s => s.UserId == userId && s.PeriodType == SnapshotPeriodType.Daily)
                .OrderByDescending(s => s.PeriodStart) 
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<FinancialIntelligenceSnapshot?>GetByMonthAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialIntelligenceSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && 
                s.PeriodType == SnapshotPeriodType.Monthly && 
                s.PeriodStart.Month == date.Month, cancellationToken);
                
        }
        public async Task<IEnumerable<FinancialIntelligenceSnapshot>>GetMonthlyAsync(Guid userId,DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialIntelligenceSnapshots
                .Where(s => s.UserId == userId && 
                s.PeriodType == SnapshotPeriodType.Daily && 
                s.PeriodStart.Month == date.Month)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<FinancialIntelligenceSnapshot?>GetByYearAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default) 
        {
            return await context.FinancialIntelligenceSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && 
                s.PeriodType == SnapshotPeriodType.Yearly && 
                s.PeriodStart.Year == date.Year, cancellationToken);
        }
        public async Task<IEnumerable<FinancialIntelligenceSnapshot>>GetYearlyAsync(Guid userId,  DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialIntelligenceSnapshots
                .Where(s => s.UserId == userId &&
                s.PeriodType == SnapshotPeriodType.Monthly &&
                date.Year == date.Year)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
