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

        public async Task<List<Budget>> ConvertBudgetToBaseCurrencyAsync(Guid userId, List<Budget> budgets)
        {
            ArgumentNullException.ThrowIfNull(budgets);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");

            foreach (Budget budget in budgets)
            {
                if (budget?.Amount == null) continue;

                decimal amount = budget.Amount.Amount;
                if (budget.Amount.Currency?.Code != null &&
                    !string.Equals(budget.Amount.Currency.Code, baseCurrency.Currency.Code, StringComparison.OrdinalIgnoreCase))
                {
                    var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(budget.Amount.Currency.Code, baseCurrency.Currency.Code);
                    amount = budget.Amount.Amount * exchangeRate;
                }

                var currency = new Currency(baseCurrency.Currency.Code, baseCurrency.Currency.Symbol);
                budget.Amount = new Money(amount, currency);
            }
            return budgets;
        }

        public async Task<Money> ConvertToBaseCurrencyAsync(Guid userId, Money money)
        {
            ArgumentNullException.ThrowIfNull(money);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var baseCurrency = await userCurrenciesRepository.GetDefaultCurrencyAsync(userId) ?? throw new NotFoundException("User has no base currency yet!");

            decimal amount = money.Amount;
            if (money.Currency?.Code != null &&
                !string.Equals(money.Currency.Code, baseCurrency.Currency.Code, StringComparison.OrdinalIgnoreCase))
            {
                var exchangeRate = await exchangeRateApiService.GetEchangeRateAsync(money.Currency.Code, baseCurrency.Currency.Code);
                amount = money.Amount * exchangeRate;
            }

            var currency = new Currency(baseCurrency.Currency.Code, baseCurrency.Currency.Symbol);
            return new Money(amount, currency);
        }
    }
}
