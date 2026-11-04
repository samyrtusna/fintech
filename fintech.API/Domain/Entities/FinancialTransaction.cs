using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Domain.Entities
{
    public class FinancialTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public FinancialType Type { get; set; }
        public Money Amount { get; set; } = null!;
        public decimal ExchangeRate { get; set; } 
        public Money BaseAmount { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public bool IsEssential { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;  
        public User User { get; set; } = null!;  
        public Category Category { get; set; } = null!;
        
    }
}
