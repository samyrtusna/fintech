using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class ExchangeRateRepository(AppDbContext context) : GenericRepository<ExchangeRate>(context), IExchangeRateRepository
    {
        public async Task<ExchangeRate?> GetLatestAsync(string fromCurrency, string toCurrency)
        {
            return await context.ExchangeRates
                .Where(er => er.FromCurrency == fromCurrency && er.ToCurrency == toCurrency)
                .OrderByDescending(er => er.Date)
                .FirstOrDefaultAsync(); 
        }
    }
}
  