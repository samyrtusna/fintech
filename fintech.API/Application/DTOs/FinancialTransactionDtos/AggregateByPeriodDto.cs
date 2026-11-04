using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class AggregateByPeriodDto 
    { 
        public DateTime Period { get; set; }
        public FinancialType Type { get; set; } 
        public string? CategoryName { get; set; }
        public decimal Amount { get; set; }
    }
}
