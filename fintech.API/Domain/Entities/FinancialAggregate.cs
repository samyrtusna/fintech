using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;


namespace fintech.API.Domain.Entities
{
    public class FinancialAggregate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Period Period { get; set; } = null!;
        public AggregateType? PeriodType { get; set; }
        public FinancialType? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public decimal? Variation { get; set; }
    }
}
