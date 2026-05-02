using fintech.Application.DTOs.CategoryDtos;
using fintech.Domain.Entities;
using fintech.Domain.Enums;

namespace fintech.Application.DTOs.TransactionDtos
{
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal BaseAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsEssential { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string CategoryName { get; set; } = null!;
    }
}
