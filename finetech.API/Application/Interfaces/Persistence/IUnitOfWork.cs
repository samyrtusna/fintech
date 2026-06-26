using fintech.API.Application.Interfaces.Repositories;

namespace fintech.API.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IFinancialIntelligenceSnapshotRepository FinancialIntelligenceSnapshots { get; }
        IFinancialTransactionRepository FinancialTransactions { get; }
        IFinancialIndicatorRepository FinancialIndicators { get; } 
        IRefreshTokenRepository RefreshTokens { get; }
        IUserCategorySettingRepository UserCategorySettings { get; }
        IUserRepository Users { get; }

        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
