using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;


namespace fintech.API.Application.Helpers
{
    public class CurrencyConversions(IUserCurrenciesRepositoy userCurrenciesRepository, IExchangeRateApiService exchangeRateApiService): ICurrencyConversions
    {
        public async Task<List<FinancialTransaction>> ConvertToBaseCurrency (Guid userId, List<FinancialTransaction> financialTransactions)
        {
            ArgumentNullException.ThrowIfNull(financialTransactions);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");
            foreach(FinancialTransaction transaction in financialTransactions) 
            {
                if (!string.Equals(transaction.BaseCurrency, baseCurrency.CurrencyCode, StringComparison.OrdinalIgnoreCase))
                {
                    var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(transaction.Currency, baseCurrency.CurrencyCode);
                    transaction.BaseAmount = transaction.Amount * exchangeRate;
                    transaction.BaseCurrency = baseCurrency.CurrencyCode;
                    transaction.BaseCurrencySymbol = baseCurrency.CurrencySymbol; 

                }
            }

            return financialTransactions;
        }
    }
}
