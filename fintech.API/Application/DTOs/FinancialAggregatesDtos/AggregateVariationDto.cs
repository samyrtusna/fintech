namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class AggregateVariationDto
    {
        public decimal IncomeVariation { get; set; }
        public decimal ExpensesVariation { get; set; }
        public decimal SavingsVariation { get; set; }
        public decimal InvestmentVariation { get; set; }
        public decimal DebtVariation { get; set; }
        public decimal NetBalanceVariation { get; set; }
        public decimal NetCashFlowVariation { get; set; }
    }
}
