using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialIndicatorsDtos
{
    public class FinancialIndicatorsResponseDto 
    {
        public Guid UserId { get; set; }
        public Guid SnapshotId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public SnapshotPeriodType PeriodType { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal EssentialExpensesRatio { get; set; }
        public decimal ExpensesToIncomeRatio { get; set; }
        public decimal InvestmentRatio { get; set; }
        public decimal DebtToIncomeRatio { get; set; }
        public decimal CashRetentionRatio { get; set; }
        public decimal DebtRepaymentRatio { get; set; }
        public decimal InterestBurdenRatio { get; set; }
    }
}
