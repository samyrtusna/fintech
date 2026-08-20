using fintech.API.Domain.Enums;

namespace fintech.API.Domain.Entities
{
    public class FinancialTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public FinancialType Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public decimal ExchangeRate { get; set; } 
        public decimal BaseAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsEssential { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;  
        public User User { get; set; } = null!;  
        public Category Category { get; set; } = null!;
        //TODO: Add BaseCurrency property to store the user's base currency at the time of transaction for historical accuracy.
    }
}
