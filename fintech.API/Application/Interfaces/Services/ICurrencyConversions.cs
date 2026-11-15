using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Interfaces.Services
{
    public interface ICurrencyConversions
    {
        Task<List<FinancialTransaction>> ConvertTransactionBaseCurrencyAsync(Guid userId, List<FinancialTransaction> financialTransactions);
        Task<List<FinancialAggregate>> ConvertAggregateBaseCurrencyAsync(Guid userId, List<FinancialAggregate> financialAggregates);
        Task<List<Budget>> ConvertBudgetToBaseCurrencyAsync(Guid userId, List<Budget> budgets);
        Task<Money> ConvertToBaseCurrencyAsync(Guid userId, Money money);
    }
}

