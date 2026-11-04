using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class FinancialTransactionResponseDto
    {
        public Guid Id { get; set; } 
        public Guid CategoryId { get; set; }
        public FinancialType Type { get; set; }
        public Money Amount { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public Money BaseAmount { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public bool IsEssential { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string CategoryName { get; set; } = null!;  
    }
}
