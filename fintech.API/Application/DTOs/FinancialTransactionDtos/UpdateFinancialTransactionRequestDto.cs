namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class UpdateFinancialTransactionRequestDto
    {
        public string? Description { get; set; }
        public bool? IsEssential { get; set; } 
    }
}
   