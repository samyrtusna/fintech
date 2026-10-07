using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IUserCurrenciesRepositoy : IGenericRepository<UserCurrency>
    {
        Task<IEnumerable<UserCurrency>> GetUserCurrenciesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserCurrency?> GetDefaultCurrencyAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
