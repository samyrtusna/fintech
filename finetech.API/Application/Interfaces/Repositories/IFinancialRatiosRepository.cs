using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IFinancialRatiosRepository : IGenericRepository<FinancialTransaction> 
    {
        Task<decimal> GetTotalIncomePerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalIncomePerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalExpensePerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalExpensePerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalInvestmentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalInvestmentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSavingsPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSavingsPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalContractedLoanPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalContractedLoanPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalInterestPaymentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalInterestPaymentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPrincipalRepaymentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalPrincipalRepaymentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    }
}
