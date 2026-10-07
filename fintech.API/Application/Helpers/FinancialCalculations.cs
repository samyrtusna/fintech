using fintech.API.Application.Interfaces.Services;

namespace fintech.API.Application.Helpers
{ 
    public class FinancialCalculations : IFinancialCalculations
    {
        public decimal GetNetBalance(decimal totalIncome, decimal totalExpense)
        {
            return totalIncome - totalExpense;
        }

        public decimal GetNetCashFlow(decimal totalIncome, decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal ContractedLoans, decimal interestPayment, decimal principalRepayment)
        {
            return totalIncome + ContractedLoans - totalExpense - totalInvestment - totalSavings - interestPayment - principalRepayment;
        }

        public decimal GetTotalInflow(decimal totalIncome, decimal ContractedLoans)
        {
            return totalIncome + ContractedLoans;
        }

        public decimal GetTotalOutflow(decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal interestPayment, decimal principalRepayment)
        {
            return totalExpense + totalInvestment + totalSavings + interestPayment + principalRepayment;
        }

        public decimal GetSavingsRate (decimal totalIncome, decimal totalSavings )
        {
            return totalIncome == 0 ? 0 : (totalSavings / totalIncome) * 100;
        }

        public decimal GetEssentialExpensesRatio (decimal totalIncome, decimal totalEssentialExpenses) 
        {
            return totalIncome == 0 ? 0 : (totalEssentialExpenses / totalIncome) * 100; 
        }

        public decimal GetExpensesRatio (decimal totalIncome, decimal totalExpenses) 
        { 
            return totalIncome == 0 ? 0 : (totalExpenses / totalIncome) * 100;
        } 

        public decimal GetInvestmentRatio (decimal totalIncome, decimal totalInvestments) 
        {
            return totalIncome == 0 ? 0 : (totalInvestments / totalIncome) * 100; 
        }
         
        public decimal GetDebtRatio (decimal totalIncome, decimal totalContractedDebts) 
        {
            return totalIncome == 0 ? 0 : (totalContractedDebts / totalIncome) * 100; 
        }

        public decimal GetDebtPaymentRatio(decimal totalIncome, decimal totalPrincipalPayment, decimal totalInterestPayment)
        {
            return totalIncome == 0 ? 0 : ((totalPrincipalPayment + totalInterestPayment) / totalIncome) * 100;
        }

        public decimal GetInterestBurdenRatio (decimal totalIncome, decimal totalPaidInterests) 
        {
            return totalIncome == 0 ? 0 : (totalPaidInterests / totalIncome) * 100; 
        }

        public decimal GetIndicatorGrowth(decimal previousValue, decimal currentValue)
        {
            return previousValue == 0 ? 0 : ((currentValue - previousValue) / previousValue) * 100;
        }
    } 
}
