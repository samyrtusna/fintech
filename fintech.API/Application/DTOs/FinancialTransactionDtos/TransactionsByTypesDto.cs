using fintech.API.Domain.Entities;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class TransactionsByTypesDto
    {
        public List<FinancialTransaction> IncomeTransactions { get; init; } = [];
        public List<FinancialTransaction> ExpenseTransactions { get; init; } = [];
        public List<FinancialTransaction> InvestmentTransactions { get; init; } = [];
        public List<FinancialTransaction> SavingTransactions { get; init; } = [];
        public List<FinancialTransaction> ContractedLoanTransactions { get; init; } = [];
        public List<FinancialTransaction> InterestPaymentTransactions { get; init; } = [];
        public List<FinancialTransaction> PrincipalRepaymentTransactions { get; init; } = [];
    }
}
