using fintech.API.Application.Interfaces.Repositories;

namespace fintech.API.Application.Interfaces.Persistence
{
    public interface IUnitOfWork 
    {
        ICategoryRepository Categories { get; }
        IFinancialTransactionRepository FinancialTransactions { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IUserCategorySettingRepository UserCategorySettings { get; }
        IUserRepository Users { get; }

        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
