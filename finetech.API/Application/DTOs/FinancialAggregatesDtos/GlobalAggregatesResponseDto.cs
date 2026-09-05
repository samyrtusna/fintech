namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class GlobalAggregatesResponseDto
    {
        public DateTime Date { get; set; }
        public decimal TotalCashFlow { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal TotalSavings { get; set; }
        public decimal TotalActiveDebt { get; set; }
    }
}
