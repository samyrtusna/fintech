using fintech.API.Application.DTOs.FinancialIndicatorsDtos;
using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;
using fintech.API.Application.Interfaces.Persistence;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Interfaces.Services.IFinancialIndicatorServices;
using fintech.API.Domain.Entities;


namespace fintech.API.Application.Services.FinancialIndicatorsServices
{
    public class FinancialIndicatorsService(IFinancialCalculations financialCalculations, IUnitOfWork unitOfWork) : IFinancialIndicatorsService
    {
        public FinancialIndicatorsResponseDto ComputeFinancialIndicators(FinancialIntelligenceSnapshotResponseDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var savingsRate = financialCalculations.SavingsRate(dto.NetSavings, dto.TotalIncomes);
            var essentialExpensesRatio = financialCalculations.EssentialExpensesRatio(dto.TotalEssentialExpenses, dto.TotalIncomes); 
            var expensesToIncomeRatio = financialCalculations.ExpensesToIncomeRatio(dto.TotalExpenses, dto.TotalIncomes);
            var investmentRatio = financialCalculations.InvestmentRatio(dto.TotalInvestments, dto.TotalIncomes);
            var debtToIncomeRatio = financialCalculations.DebtToIncomeRatio(dto.TotalContractedDebts, dto.TotalIncomes);
            var cashRetentionRatio = financialCalculations.CashRetentionRatio(dto.CashFlow, dto.TotalIncomes);
            var debtRepaymentRatio = financialCalculations.DebtRepaymentRatio(dto.TotalRepayedDebts, dto.TotalContractedDebts);
            var interestBurdenRatio = financialCalculations.InterestBurdenRatio(dto.TotalPaidInterests, dto.TotalIncomes);

            return  new FinancialIndicatorsResponseDto 
            {
                UserId = dto.UserId,
                SnapshotId = dto.Id,
                PeriodStart = dto.PeriodStart,
                PeriodEnd = dto.PeriodEnd,
                PeriodType = dto.PeriodType,
                SavingsRate = savingsRate,
                EssentialExpensesRatio = essentialExpensesRatio,
                ExpensesToIncomeRatio = expensesToIncomeRatio,
                InvestmentRatio = investmentRatio,
                DebtToIncomeRatio = debtToIncomeRatio,
                CashRetentionRatio = cashRetentionRatio,
                DebtRepaymentRatio = debtRepaymentRatio,
                InterestBurdenRatio = interestBurdenRatio
            };
        }

        public async Task CreateFinancialIndicators(FinancialIntelligenceSnapshotResponseDto dto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var indicators = ComputeFinancialIndicators(dto);

            await unitOfWork.FinancialIndicators.AddRangeAsync([
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Savings Rate", Value = indicators.SavingsRate },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Essential Expenses Ratio", Value = indicators.EssentialExpensesRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Expenses to Income Ratio", Value = indicators.ExpensesToIncomeRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Investment Ratio", Value = indicators.InvestmentRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Debt to Income Ratio", Value = indicators.DebtToIncomeRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Cash Retention Ratio", Value = indicators.CashRetentionRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Debt Repayment Ratio", Value = indicators.DebtRepaymentRatio },
                new FinancialIndicator {UserId = indicators.UserId, SnapshotId = indicators.SnapshotId, PeriodStart = indicators.PeriodStart, PeriodEnd = indicators.PeriodEnd, PeriodType = indicators.PeriodType, Name = "Interest Burden Ratio", Value = indicators.InterestBurdenRatio }
            ], cancellationToken);
        }

        // Implement another method for retrieving indicators if needed
    }
}
