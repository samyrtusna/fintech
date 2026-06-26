using fintech.API.Domain.Enums;

namespace fintech.API.Domain.Entities
{
    public class FinancialIndicator
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid SnapshotId { get; set; } 
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public SnapshotPeriodType PeriodType { get; set; }
        public String Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Description { get; set; } = String.Empty;
        public FinancialIntelligenceSnapshot SnapShot { get; set; } = null!;
    }
}
