using fintech.API.Application.Interfaces.Services;

namespace fintech.API.Application.Helpers
{ 
    public class FinancialCalculations : IFinancialCalculations
    {
        
        public decimal SavingsRate (decimal netSavings, decimal totalIncome)
        {
            return totalIncome == 0 ? 0 : (netSavings / totalIncome) * 100;
        }

        public decimal EssentialExpensesRatio (decimal totalEssentialExpenses, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (totalEssentialExpenses / totalIncome) * 100;
        }

        public decimal ExpensesToIncomeRatio (decimal totalExpenses, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (totalExpenses / totalIncome) * 100;
        } 

        public decimal InvestmentRatio (decimal totalInvestments, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (totalInvestments / totalIncome) * 100;
        }

        public decimal DebtToIncomeRatio (decimal totalContractedDebts, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (totalContractedDebts / totalIncome) * 100;
        }

        public decimal CashRetentionRatio (decimal cashFlow, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (cashFlow / totalIncome) * 100;
        }

        public decimal DebtRepaymentRatio (decimal totalRepayedDebts, decimal totalContractedDebts) 
        {
            return totalContractedDebts == 0 ? 0 : (totalRepayedDebts / totalContractedDebts) * 100;
        }

        public decimal InterestBurdenRatio (decimal totalPaidInterests, decimal totalIncome) 
        {
            return totalIncome == 0 ? 0 : (totalPaidInterests / totalIncome) * 100;
        }
    } 
}
