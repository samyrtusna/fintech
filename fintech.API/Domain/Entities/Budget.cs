using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public BudgetType Type { get; set; }
        public Money Amount { get; set; } = null!;
        public Period Period { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Category? Category { get; set; }
    }
}
