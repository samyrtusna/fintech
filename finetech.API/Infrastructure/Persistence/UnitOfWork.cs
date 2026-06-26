using fintech.API.Application.Interfaces.Persistence;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Infrastructure.EFcore.ContextDb;

namespace fintech.API.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Categories { get; }
        public IFinancialIntelligenceSnapshotRepository FinancialIntelligenceSnapshots { get; }
        public IFinancialTransactionRepository FinancialTransactions { get; }
        public IFinancialIndicatorRepository FinancialIndicators { get; } 
        public IRefreshTokenRepository RefreshTokens { get; }
        public IUserCategorySettingRepository UserCategorySettings { get; }
        public IUserRepository Users { get; }

        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context, ICategoryRepository categories, IFinancialIntelligenceSnapshotRepository financialIntelligenceSnapshots, IFinancialTransactionRepository financialTransactions, IFinancialIndicatorRepository financialIndicators, IRefreshTokenRepository refreshTokens, IUserCategorySettingRepository userCategorySettings, IUserRepository users)
        {
            _context = context;
            Categories = categories;
            FinancialIntelligenceSnapshots = financialIntelligenceSnapshots;
            FinancialTransactions = financialTransactions;
            FinancialIndicators = financialIndicators;
            RefreshTokens = refreshTokens;
            UserCategorySettings = userCategorySettings;
            Users = users;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
