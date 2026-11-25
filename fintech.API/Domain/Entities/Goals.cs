using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Domain.Entities
{
    public class Goals
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public FinancialType? TargetType { get; set; }
        public Money TargetAmount { get; set; } = null!;
        public Period Period { get; set; } = null!;
        public decimal GoalProgress { get; set; } = 0; 
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category? Category { get; set; }
    }
}
