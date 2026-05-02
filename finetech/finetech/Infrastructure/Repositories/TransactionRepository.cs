using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.Infrastructure.Repositories
{
    public class TransactionRepository(AppDbContext context) : GenericRepository<FinancialTransaction>(context), ITransactionRepository
    {
        public async Task<FinancialTransaction?> GetAsync(Guid id)
        {
            return await context.FinancialTransactions
                .Where(t => t.Id == id && !t.IsDeleted)
                .Include(t => t.Category)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FinancialTransaction>> GetByCategoryIdAsync(Guid categoryId)
        { 
            return await context.FinancialTransactions
                .Where(t => t.CategoryId == categoryId && !t.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
    }       
}
