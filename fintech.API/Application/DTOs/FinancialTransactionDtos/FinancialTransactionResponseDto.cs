using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class FinancialTransactionResponseDto
    {
        public Guid Id { get; set; } 
        public Guid CategoryId { get; set; }
        public FinancialType Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal BaseAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsEssential { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string CategoryName { get; set; } = null!;  
    }
}
