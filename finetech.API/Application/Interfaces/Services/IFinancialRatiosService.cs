namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialRatiosService
    {
        decimal GetNetBalance(decimal totalIncome, decimal totalExpense);
        decimal GetNetCashFlow(decimal totalIncome, decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal ContractedLoans, decimal interestPayment, decimal principalRepayment);
        decimal GetTotalOutflow(decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal interestPayment, decimal principalRepayment);
        decimal GetTotalInflow(decimal totalIncome, decimal ContractedLoans);
        decimal GetTotalActiveDebt(decimal totalContractedLoans, decimal totalPrincipalRepayment);
        decimal GetExpenseRatio(decimal totalIncome, decimal totalExpense);
        decimal GetNonEssentialExpenseRatio(decimal totalExpense, decimal nonEssentialExpenses);
        decimal GetInvestmentRate(decimal totalIncome, decimal totalInvestment);
        decimal GetSavingsRate(decimal totalIncome, decimal totalSavings);
        decimal GetIndicatorGrowth(decimal previousValue, decimal currentValue);
        decimal GetDebtPaymentRatio(decimal totalIncome, decimal totalPrincipalPayment,decimal totalInterestPayment);
    }
}
