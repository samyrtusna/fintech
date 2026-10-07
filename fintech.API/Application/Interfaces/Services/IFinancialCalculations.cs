namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialCalculations
    {
        decimal GetNetBalance(decimal totalIncome, decimal totalExpense);
        decimal GetNetCashFlow(decimal totalIncome, decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal ContractedLoans, decimal interestPayment, decimal principalRepayment);
        decimal GetTotalInflow(decimal totalIncome, decimal ContractedLoans);
        decimal GetTotalOutflow(decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal interestPayment, decimal principalRepayment);
        decimal GetSavingsRate(decimal totalSavings, decimal totalIncome);
        decimal GetEssentialExpensesRatio(decimal totalEssentialExpenses, decimal totalIncome);
        decimal GetExpensesRatio(decimal totalExpenses, decimal totalIncome);


        decimal GetInvestmentRatio(decimal totalInvestments, decimal totalIncome);
        decimal GetDebtRatio(decimal totalContractedDebts, decimal totalIncome);
        decimal GetDebtPaymentRatio(decimal totalIncome, decimal totalPrincipalPayment, decimal totalInterestPayment);
        decimal GetInterestBurdenRatio(decimal totalPaidInterests, decimal totalIncome);
        decimal GetIndicatorGrowth(decimal previousValue, decimal currentValue);
    }
}
