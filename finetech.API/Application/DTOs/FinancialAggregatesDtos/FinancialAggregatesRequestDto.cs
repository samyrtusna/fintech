using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialAggregatesRequestDto
    {
        public int Year { get; set; }
        public AggregateName Name { get; set; } 
        public decimal Value { get; set; }
    }
}
