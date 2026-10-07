using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialAggregatesFilterDto
    {
        public TransactionPeriod AggregatesPeriod { get; set; }
        public DateTime Date { get; set; }
    }
}
