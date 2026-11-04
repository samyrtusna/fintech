using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class AggregateVariationsValueByMonthDto
    {

        public Period Period { get; set; } = null!;
        public AggregateType? PeriodType { get; set; }
        public FinancialType? Type { get; set; }
        public Money Amount { get; set; } = null!;
    }
}
