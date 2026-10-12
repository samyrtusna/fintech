using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Services
{
    public interface ICurrencyConversions
    {
        Task<List<FinancialTransaction>> ConvertToBaseCurrency(Guid userId, List<FinancialTransaction> financialTransactions);
    }
}
