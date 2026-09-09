using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class FinancialRatiosRepository(AppDbContext context) : GenericRepository<FinancialTransaction>(context) , IFinancialRatiosRepository
    {
        public async Task<decimal> GetTotalIncomePerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.Income)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalIncomePerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.Income)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalExpensePerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.Expense)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalExpensePerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.Expense)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalInvestmentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.Investment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalInvestmentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.Investment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }


        public async Task<decimal> GetTotalSavingsPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.Savings)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalSavingsPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.Savings)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalContractedLoanPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.ContractedLoan)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalContractedLoanPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.ContractedLoan)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalInterestPaymentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.InterestPayment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalInterestPaymentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.InterestPayment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalPrincipalRepaymentPerMonth(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.TransactionDate.Month == date.Month &&
                t.Type == FinancialType.PrincipalRepayment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }

        public async Task<decimal> GetTotalPrincipalRepaymentPerYear(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.FinancialTransactions
                .Where(t => t.UserId == userId &&
                !t.IsDeleted &&
                t.TransactionDate.Year == date.Year &&
                t.Type == FinancialType.PrincipalRepayment)
                .AsNoTracking()
                .SumAsync(t => t.Amount, cancellationToken);
        }
    }
}
