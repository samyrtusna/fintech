using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Services
{
    public interface ICurrencyConversions
    {
        Task<List<FinancialTransaction>> ConvertTransactionBaseCurrencyAsync(Guid userId, List<FinancialTransaction> financialTransactions);
        Task<List<FinancialAggregate>> ConvertAggregateBaseCurrencyAsync(Guid userId, List<FinancialAggregate> financialAggregates);
    }
}

