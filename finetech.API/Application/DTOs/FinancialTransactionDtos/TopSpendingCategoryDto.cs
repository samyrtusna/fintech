namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class TopSpendingCategoryDto 
    {
        public string Category {  get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
