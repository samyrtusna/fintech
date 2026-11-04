using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;


namespace fintech.API.Application.Helpers
{
    public class CurrencyConversions(IUserCurrenciesRepositoy userCurrenciesRepository, IExchangeRateApiService exchangeRateApiService) : ICurrencyConversions
    {
        public async Task<List<FinancialTransaction>> ConvertTransactionBaseCurrencyAsync(Guid userId, List<FinancialTransaction> financialTransactions)
        {
            ArgumentNullException.ThrowIfNull(financialTransactions);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");
            foreach (FinancialTransaction transaction in financialTransactions)
            {
                if (!string.Equals(transaction?.BaseAmount?.Currency?.Code, baseCurrency.Currency.Code, StringComparison.OrdinalIgnoreCase))
                {
                    var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(transaction!.BaseAmount.Currency!.Code, baseCurrency.Currency.Code);
                    var amount = transaction.Amount.Amount * exchangeRate;
                    var currency = new Currency(baseCurrency.Currency.Code, baseCurrency.Currency.Symbol);
                    transaction.BaseAmount = new Money(amount, currency);
                }
            }
             
            return financialTransactions;
        } 

        public async Task<List<FinancialAggregate>> ConvertAggregateBaseCurrencyAsync(Guid userId, List<FinancialAggregate> financialAggregates)
        {
            ArgumentNullException.ThrowIfNull(financialAggregates);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");
            foreach (FinancialAggregate aggregate in financialAggregates)
            {
                if (!string.Equals(aggregate?.Amount?.Currency?.Code, baseCurrency.Currency.Code, StringComparison.OrdinalIgnoreCase)) 
                {
                    var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(aggregate!.Amount.Currency!.Code, baseCurrency.Currency.Code);
                    var amount = aggregate.Amount.Amount * exchangeRate;
                    var currency = new Currency(baseCurrency.Currency.Code, baseCurrency.Currency.Symbol);
                    aggregate.Amount = new Money(amount, currency);
                }
            }
            return financialAggregates;
        }
    }
}
