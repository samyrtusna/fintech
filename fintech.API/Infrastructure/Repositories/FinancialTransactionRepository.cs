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


        public async Task<List<FinancialTransaction>> GetByTypeByMonth (Guid userId, DateTime date, FinancialType type, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == type)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task<List<FinancialTransaction>> GetByTypeByYear(Guid userId, DateTime date, FinancialType type, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == type)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

    }
}
  