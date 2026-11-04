namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialCalculations
    {
        decimal GetNetBalance(decimal totalIncome, decimal totalExpense);
        decimal GetNetCashFlow(decimal totalIncome, decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal ContractedLoans, decimal interestPayment, decimal principalRepayment);
        decimal GetTotalInflow(decimal totalIncome, decimal ContractedLoans);
        decimal GetTotalOutflow(decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal interestPayment, decimal principalRepayment);
        decimal GetSavingsRate(decimal totalIncome, decimal totalSavings);
        decimal GetEssentialExpensesRatio(decimal totalIncome, decimal totalEssentialExpenses);
        decimal GetDiscretionaryExpensesRatio(decimal totalIncome, decimal totalDiscretionaryExpenses);
        decimal GetDiscretionaryToExpensesRatio(decimal totalDiscretionaryExpenses, decimal totalEssentialExpenses);
        decimal GetExpensesRatio(decimal totalIncome, decimal totalExpenses);
        decimal GetInvestmentRatio(decimal totalIncome, decimal totalInvestments);
        decimal GetDebtRatio(decimal totalIncome, decimal totalContractedDebts);
        decimal GetDebtPaymentRatio(decimal totalIncome, decimal totalPrincipalPayment, decimal totalInterestPayment);
        decimal GetInterestBurdenRatio(decimal totalIncome, decimal totalPaidInterests);
        decimal GetIndicatorGrowth(decimal previousValue, decimal currentValue);
    }
}
