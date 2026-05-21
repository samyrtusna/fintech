using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {
        Task<ExchangeRate?> GetLatestAsync(string fromCurrency, string toCurrency); 
    }
}
