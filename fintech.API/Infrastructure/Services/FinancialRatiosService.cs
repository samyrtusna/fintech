using fintech.API.Application.Interfaces.Services;

namespace fintech.API.Infrastructure.Services
{
    public class FinancialRatiosService() : IFinancialRatiosService
    {

        public decimal GetNetBalance(decimal totalIncome, decimal totalExpense)
        {
            return totalIncome - totalExpense;
        }

        public decimal GetNetCashFlow(decimal totalIncome, decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal ContractedLoans, decimal interestPayment, decimal principalRepayment)
        {
            return totalIncome + ContractedLoans - totalExpense - totalInvestment - totalSavings - interestPayment - principalRepayment;
        }

        public decimal GetTotalOutflow(decimal totalExpense, decimal totalInvestment, decimal totalSavings, decimal interestPayment, decimal principalRepayment)
        {
            return totalExpense + totalInvestment + totalSavings + interestPayment + principalRepayment;
        }

        public decimal GetTotalInflow(decimal totalIncome, decimal ContractedLoans)
        {
            return totalIncome + ContractedLoans;
        }

        public decimal GetTotalActiveDebt(decimal totalContractedLoans, decimal totalPrincipalRepayment)
        {
            return totalContractedLoans - totalPrincipalRepayment;
        }

        public decimal GetExpenseRatio(decimal totalIncome, decimal totalExpense)
        {
            if (totalIncome == 0)
            {
                return 0;
            }
            return ((totalExpense / totalIncome) * 100);
        }

        public decimal GetNonEssentialExpenseRatio(decimal totalExpense, decimal nonEssentialExpenses)
        {
            if (totalExpense == 0)
            {
                return 0;
            }
            return ((nonEssentialExpenses / totalExpense) * 100);
        }

        public decimal GetInvestmentRate(decimal totalIncome, decimal totalInvestment)
        {
            if (totalIncome == 0)
            {
                return 0;
            }
            return ((totalInvestment / totalIncome) * 100);
        }

        public decimal GetSavingsRate(decimal totalIncome, decimal totalSavings)
        {
            if (totalIncome == 0)
            {
                return 0;
            }
            return ((totalSavings / totalIncome) * 100);
        }

        public decimal GetIndicatorGrowth(decimal previousValue, decimal currentValue)
        {
            if (previousValue == 0)
            {
                return 0;
            }
            return ((currentValue - previousValue) / previousValue) * 100;
        }

        public decimal GetDebtPaymentRatio(decimal totalIncome, decimal totalPrincipalPayment, decimal totalInterestPayment)
        {
            if (totalIncome == 0)
            {
                return 0;
            }
            return ((totalPrincipalPayment + totalInterestPayment) / totalIncome) * 100;
        } 
    }
}
