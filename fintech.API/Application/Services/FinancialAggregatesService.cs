using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Services
{
    public class FinancialAggregatesService(IFinancialAggregatesRepository financialAggregatesRepository,
        IFinancialSumsRepository financialRatiosRepository,
        IFinancialCalculations financialCalculations
        ) : IFinancialAggregatesService
    {
        public async Task<GlobalAggregatesResponseDto> GetGlobalAggregates(Guid userId)
        {
            var currentYear = DateTime.UtcNow;
            var lastYear = DateTime.UtcNow.Year - 1;
            var lastYearAggregates = await financialAggregatesRepository.GetFinancialAggregatesByYearAsync(userId, lastYear);

            var currentYearIncomes = await financialRatiosRepository.GetTotalIncomePerYear(userId, currentYear);
            var currentYearExpenses = await financialRatiosRepository.GetTotalExpensePerYear(userId, currentYear);
            var currentYearInvestments = await financialRatiosRepository.GetTotalInvestmentPerYear(userId, currentYear);
            var currentYearSavings = await financialRatiosRepository.GetTotalSavingsPerYear(userId, currentYear);
            var currentYearContractedLoans = await financialRatiosRepository.GetTotalContractedLoanPerYear(userId, currentYear);
            var currentYearInterestPayments = await financialRatiosRepository.GetTotalInterestPaymentPerYear(userId, currentYear);
            var currentYearPrincipalRepayments = await financialRatiosRepository.GetTotalPrincipalRepaymentPerYear(userId, currentYear);

            var currentYearNetCashFlow = financialCalculations.GetNetCashFlow(currentYearIncomes, currentYearExpenses, currentYearInvestments, currentYearSavings, currentYearContractedLoans, currentYearInterestPayments, currentYearPrincipalRepayments);
            
            var lastYearNetCashFlow = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.NetCashFlow)?.Value ?? 0;
            var lastYearTotalInvestment = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalInvestment)?.Value ?? 0;
            var lastYearTotalSavings = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalSavings)?.Value ?? 0;
            var lastYearActiveDebt = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.ActiveDebt)?.Value ?? 0;

            var totalCashFlow = currentYearNetCashFlow + lastYearNetCashFlow;
            var totalInvestment = currentYearInvestments + lastYearTotalInvestment;
            var totalSavings = currentYearSavings + lastYearTotalSavings;
            var totalActiveDebt = currentYearContractedLoans + lastYearActiveDebt;

            return new GlobalAggregatesResponseDto
            {
                Date = DateTime.UtcNow,
                TotalCashFlow = totalCashFlow,
                TotalInvestment = totalInvestment,
                TotalSavings = totalSavings,
                TotalActiveDebt = totalActiveDebt
            };
        }

        public async Task<FinancialAggregatesResponseDto[]> GetAggregatesByFilter(Guid userId, FinancialAggregatesFilterDto dto)
        {
            if (dto.AggregatesPeriod == TransactionPeriod.Monthly)
            {
                return await GetAggregatesPerMonth(userId, dto.Date);
            }  
            else if (dto.AggregatesPeriod == TransactionPeriod.Yearly)
            {
                return await GetAggregatesPerYear(userId, dto.Date);
            }
            else
            {
                throw new ArgumentException("Invalid period. Must be 'month' or 'year'.");
            }
        }

        private async Task<FinancialAggregatesResponseDto[]> GetAggregatesPerMonth(Guid userId, DateTime date)
        {

            var totalIncomes = await financialRatiosRepository.GetTotalIncomePerMonth(userId, date);
            var totalExpenses = await financialRatiosRepository.GetTotalExpensePerMonth(userId, date);
            var totalInvestment = await financialRatiosRepository.GetTotalInvestmentPerMonth(userId, date);
            var totalSavings = await financialRatiosRepository.GetTotalSavingsPerMonth(userId, date);
            var totalContractedLoans = await financialRatiosRepository.GetTotalContractedLoanPerMonth(userId, date);
            var totalInterestPayments = await financialRatiosRepository.GetTotalInterestPaymentPerMonth(userId, date);
            var totalPrincipalRepayments = await financialRatiosRepository.GetTotalPrincipalRepaymentPerMonth(userId, date);

            var netBalance = financialCalculations.GetNetBalance(totalIncomes, totalExpenses);
            var netCashFlow = financialCalculations.GetNetCashFlow(totalIncomes, totalExpenses, totalInvestment, totalSavings, totalContractedLoans, totalInterestPayments, totalPrincipalRepayments);
            var totalInflow = financialCalculations.GetTotalInflow(totalIncomes, totalContractedLoans);
            var totalOutflow = financialCalculations.GetTotalOutflow(totalExpenses, totalInvestment, totalSavings, totalInterestPayments, totalPrincipalRepayments);
            var savingsRate = financialCalculations.GetSavingsRate(totalSavings, totalIncomes);
            var essentialExpensesRatio = financialCalculations.GetEssentialExpensesRatio(totalExpenses, totalIncomes);
            var expensesRatio = financialCalculations.GetExpensesRatio(totalExpenses, totalIncomes);
            var investmentRatio = financialCalculations.GetInvestmentRatio(totalInvestment, totalIncomes);
            var debtRatio = financialCalculations.GetDebtRatio(totalContractedLoans, totalIncomes);
            var debtPaymentRatio = financialCalculations.GetDebtPaymentRatio(totalIncomes, totalPrincipalRepayments, totalInterestPayments);
            var interestBurdenRatio = financialCalculations.GetInterestBurdenRatio(totalInterestPayments, totalIncomes);
            return [
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalIncome, Value = totalIncomes },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalExpense, Value = totalExpenses },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalInvestment, Value = totalInvestment },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalSavings, Value = totalSavings },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalContractedLoans, Value = totalContractedLoans },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalInterestPayments, Value = totalInterestPayments },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalPrincipalRepayments, Value = totalPrincipalRepayments },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.NetBalance, Value = netBalance },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.NetCashFlow, Value = netCashFlow },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalInflow, Value = totalInflow },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.TotalOutflow, Value = totalOutflow },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.SavingsRate, Value = savingsRate },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.EssentialExpensesRatio, Value = essentialExpensesRatio },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.ExpensesRatio, Value = expensesRatio },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.InvestmentRatio, Value = investmentRatio },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.DebtRatio, Value = debtRatio },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.DebtPaymentRatio, Value = debtPaymentRatio },
                new() { Year = date.Year, Month = date.Month, Name = AggregateName.InterestBurdenRatio, Value =
 interestBurdenRatio }
            ]; 
        }

        private async Task<FinancialAggregatesResponseDto[]> GetAggregatesPerYear(Guid userId, DateTime date)
        {
            var totalIncomes = await financialRatiosRepository.GetTotalIncomePerYear(userId, date);
            var totalExpenses = await financialRatiosRepository.GetTotalExpensePerYear(userId, date);
            var totalInvestment = await financialRatiosRepository.GetTotalInvestmentPerYear(userId, date);
            var totalSavings = await financialRatiosRepository.GetTotalSavingsPerYear(userId, date);
            var totalContractedLoans = await financialRatiosRepository.GetTotalContractedLoanPerYear(userId, date);
            var totalInterestPayments = await financialRatiosRepository.GetTotalInterestPaymentPerYear(userId, date);
            var totalPrincipalRepayments = await financialRatiosRepository.GetTotalPrincipalRepaymentPerYear(userId, date);

            var netBalance = financialCalculations.GetNetBalance(totalIncomes, totalExpenses);
            var netCashFlow = financialCalculations.GetNetCashFlow(totalIncomes, totalExpenses, totalInvestment, totalSavings, totalContractedLoans, totalInterestPayments, totalPrincipalRepayments);
            var totalInflow = financialCalculations.GetTotalInflow(totalIncomes, totalContractedLoans);
            var totalOutflow = financialCalculations.GetTotalOutflow(totalExpenses, totalInvestment, totalSavings, totalInterestPayments, totalPrincipalRepayments);
            var savingsRate = financialCalculations.GetSavingsRate(totalSavings, totalIncomes);
            var essentialExpensesRatio = financialCalculations.GetEssentialExpensesRatio(totalExpenses, totalIncomes);
            var expensesRatio = financialCalculations.GetExpensesRatio(totalExpenses, totalIncomes);
            var investmentRatio = financialCalculations.GetInvestmentRatio(totalInvestment, totalIncomes);
            var debtRatio = financialCalculations.GetDebtRatio(totalContractedLoans, totalIncomes);
            var debtPaymentRatio = financialCalculations.GetDebtPaymentRatio(totalIncomes, totalPrincipalRepayments, totalInterestPayments);
            var interestBurdenRatio = financialCalculations.GetInterestBurdenRatio(totalInterestPayments, totalIncomes);
            return [
            
                new() { Year = date.Year,  Name = AggregateName.TotalIncome, Value = totalIncomes },
                new() { Year = date.Year,  Name = AggregateName.TotalExpense, Value = totalExpenses },
                new() { Year = date.Year,  Name = AggregateName.TotalInvestment, Value = totalInvestment },
                new() { Year = date.Year,  Name = AggregateName.TotalSavings, Value = totalSavings },
                new() { Year = date.Year,  Name = AggregateName.TotalContractedLoans, Value = totalContractedLoans },
                new() { Year = date.Year,  Name = AggregateName.TotalInterestPayments, Value = totalInterestPayments },
                new() { Year = date.Year,  Name = AggregateName.TotalPrincipalRepayments, Value = totalPrincipalRepayments },
                new() { Year = date.Year,  Name = AggregateName.NetBalance, Value = netBalance },
                new() { Year = date.Year,  Name = AggregateName.NetCashFlow, Value = netCashFlow },
                new() { Year = date.Year,  Name = AggregateName.TotalInflow, Value = totalInflow },
                new() { Year = date.Year,  Name = AggregateName.TotalOutflow, Value = totalOutflow },
                new() { Year = date.Year,  Name = AggregateName.SavingsRate, Value = savingsRate },
                new() { Year = date.Year,  Name = AggregateName.EssentialExpensesRatio, Value = essentialExpensesRatio },
                new() { Year = date.Year,  Name = AggregateName.ExpensesRatio, Value = expensesRatio },
                new() { Year = date.Year,  Name = AggregateName.InvestmentRatio, Value = investmentRatio },
                new() { Year = date.Year,  Name = AggregateName.DebtRatio, Value = debtRatio },
                new() { Year = date.Year,  Name = AggregateName.DebtPaymentRatio, Value = debtPaymentRatio },
                new() { Year = date.Year,  Name = AggregateName.InterestBurdenRatio, Value =
 interestBurdenRatio }
            ];
        }
    }
}
