using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialTransactionRepository(AppDbContext context) : GenericRepository<FinancialTransaction>(context), IFinancialTransactionRepository
    {
        public async Task<FinancialTransaction?> GetAsync(Guid id)
        {
            return await context.FinancialTransactions
                .Where(t => t.Id == id && !t.IsDeleted)
                .Include(t => t.Category)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(Guid categoryId)
        {
            return await context.FinancialTransactions
                .Where(t => t.CategoryId == categoryId && !t.IsDeleted)
                .Include(t => t.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialTransaction>> GetByDayAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.TransactionDate.Day == date.Day)
                .Include(t => t.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task<List<FinancialTransaction>> GetByMonth (Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions 
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month)
                .Include(t => t.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task<List<FinancialTransaction>> GetByYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AggregateByPeriodDto>> GetByTypeByMonth (Guid userId, FinancialType type, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                t.Type == type &&
                !t.IsDeleted)
                .AsNoTracking()
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new AggregateByPeriodDto
                {
                    Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Type = type,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AggregateByPeriodDto>>GetByCategoryByMonth (Guid userId, string categoryName, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                t.Category.Name == categoryName &&
                !t.IsDeleted)
                .AsNoTracking()
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new AggregateByPeriodDto
                {
                    Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Type = g.First().Type,
                    CategoryName = g.First().Category.Name,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);
        }
    }
}
 