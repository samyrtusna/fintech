using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Services
{
    public class FinancialAggregatesService(IFinancialAggregatesRepository financialAggregatesRepository,
        IFinancialRatiosRepository financialRatiosRepository,
        IFinancialRatiosService financialRatiosService
        )
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

            var currentYearNetCashFlow = financialRatiosService.GetNetCashFlow(currentYearIncomes, currentYearExpenses, currentYearInvestments, currentYearSavings, currentYearContractedLoans, currentYearInterestPayments, currentYearPrincipalRepayments);
            
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

        public async Task<int> GetAggregatesPerMonth()
        {
            return 0;
        }

        public async Task<int> GetAggregatesPerYear()
        {
            return 0;
        }
    }
}
//TODO : Implement the logic for GetAggregatesPerMonth and GetAggregatesPerYear methods to calculate monthly and yearly aggregates based on financial data.