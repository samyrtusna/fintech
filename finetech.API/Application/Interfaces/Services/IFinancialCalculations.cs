namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialCalculations
    {
        decimal SavingsRate(decimal netSavings, decimal totalIncome);
        decimal EssentialExpensesRatio(decimal totalEssentialExpenses, decimal totalIncome);
        decimal ExpensesToIncomeRatio(decimal totalExpenses, decimal totalIncome);
        decimal InvestmentRatio(decimal totalInvestments, decimal totalIncome);
        decimal DebtToIncomeRatio(decimal totalContractedDebts, decimal totalIncome);
        decimal CashRetentionRatio(decimal cashFlow, decimal totalIncome);
        decimal DebtRepaymentRatio(decimal totalRepayedDebts, decimal totalContractedDebts);
        decimal InterestBurdenRatio(decimal totalPaidInterests, decimal totalIncome);
    }
}
