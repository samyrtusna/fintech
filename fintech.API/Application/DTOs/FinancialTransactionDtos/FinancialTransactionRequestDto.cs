namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class FinancialTransactionRequestDto
    {
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string? Description { get; set; } 
        public bool? IsEssential { get; set; }  
        public DateTime? TransactionDate { get; set; }
    }    
}
 