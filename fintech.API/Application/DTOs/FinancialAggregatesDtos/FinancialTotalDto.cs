using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialTotalDto
    {
        public FinancialType? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public TotalType? Filter { get; set; }
        public decimal Amount { get; set; }
    }  
}
