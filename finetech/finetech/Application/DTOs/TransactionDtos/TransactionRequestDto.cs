using fintech.Domain.Entities;
using fintech.Domain.Enums;

namespace fintech.Application.DTOs.TransactionDtos
{
    public class TransactionRequestDto
    {
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public string? Description { get; set; } 
        public bool? IsEssential { get; set; }
        public DateTime? TransactionDate { get; set; }
    }  
}
