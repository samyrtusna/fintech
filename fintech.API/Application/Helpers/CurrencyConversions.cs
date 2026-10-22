using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;


namespace fintech.API.Application.Helpers
{
    public class CurrencyConversions(IUserCurrenciesRepositoy userCurrenciesRepository, IExchangeRateApiService exchangeRateApiService) : ICurrencyConversions
    {
        public async Task<List<FinancialTransaction>> ConvertTransactionBaseCurrency(Guid userId, List<FinancialTransaction> financialTransactions)
        {
            ArgumentNullException.ThrowIfNull(financialTransactions);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");
            foreach (FinancialTransaction transaction in financialTransactions)
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

        public async Task<List<FinancialAggregate>> ConvertAggregateBaseCurrency(Guid userId, List<FinancialAggregate> financialAggregates)
        {
            ArgumentNullException.ThrowIfNull(financialAggregates);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");
            foreach (FinancialAggregate aggregate in financialAggregates)
            {
                if (!string.Equals(aggregate.Currency, baseCurrency.CurrencyCode, StringComparison.OrdinalIgnoreCase))
                {
                    var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(aggregate.Currency, baseCurrency.CurrencyCode);
                    aggregate.Value = aggregate.Value * exchangeRate;
                    aggregate.Currency = baseCurrency.CurrencyCode;
                }
            }
            return financialAggregates;
        }
    }
}
