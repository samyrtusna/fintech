namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialTotalsDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal TotalSavings { get; set; }
        public decimal TotalContractedLoans { get; set; }
        public decimal TotalInterestPayments { get; set; }
        public decimal TotalPrincipalRepayments { get; set; }

    } 
}
