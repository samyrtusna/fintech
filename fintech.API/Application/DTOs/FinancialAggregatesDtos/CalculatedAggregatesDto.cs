namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class CalculatedAggregatesDto
    {
        public decimal NetBalance { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal TotalInflow { get; set; }
        public decimal TotalOutflow { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal EssentialExpensesRatio { get; set; }
        public decimal ExpensesRatio { get; set; }
        public decimal InvestmentRatio { get; set; }
        public decimal DebtRatio { get; set; }
        public decimal DebtPaymentRatio { get; set; }
        public decimal InterestBurdenRatio { get; set; } 
    }
}
