namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class AggregatesByTypeDto
    {
        public decimal TotalIncome { get; set; } 
        public decimal TotalExpenses { get; set; }
        public decimal TotalInvestments { get; set; }
        public decimal TotalSavings { get; set; } 
        public decimal TotalContractedLoans { get; set; } 
        public decimal TotalInterestPayments { get; set; }
        public decimal TotalPrincipalRepayments { get; set; }
    }
}
