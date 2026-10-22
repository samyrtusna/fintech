using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Services
{
    public interface ICurrencyConversions
    {
        Task<List<FinancialTransaction>> ConvertTransactionBaseCurrency(Guid userId, List<FinancialTransaction> financialTransactions);
        Task<List<FinancialAggregate>> ConvertAggregateBaseCurrency(Guid userId, List<FinancialAggregate> financialAggregates);
    }
}

