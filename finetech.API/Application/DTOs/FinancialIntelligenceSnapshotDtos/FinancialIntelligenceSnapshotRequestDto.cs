using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos
{
    public class FinancialIntelligenceSnapshotRequestDto
    {
        public Guid? UserId { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public SnapshotPeriodType? PeriodType { get; set; }
        public decimal TotalIncomes { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance { get; set; }
        public decimal CashFlow { get; set; }
        public decimal TotalNetCashFlow { get; set; }
        public decimal NetSavings { get; set; }
        public decimal TotalInvestments { get; set; }
        public decimal TotalEssentialExpenses { get; set; }
        public decimal TotalContractedDebts { get; set; }
        public decimal TotalRepayedDebts { get; set; }
        public decimal TotalPaidInterests { get; set; }
        public string TopSpendingCategory { get; set; } = string.Empty;
        public decimal TopSpendingCategoryAmount { get; set; }
        public int TransactionsCount { get; set; }
        public decimal AvgTransactionAmount { get; set; }
    }
}
