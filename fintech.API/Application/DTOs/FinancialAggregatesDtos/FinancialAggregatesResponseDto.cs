using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialAggregatesResponseDto
    {
        public DateTime Date { get; set; } 
        public List<FinancialTotalDto> Totals { get; set; } = new List<FinancialTotalDto>();
        public CalculatedAggregatesDto Aggregates { get; set; } = new CalculatedAggregatesDto();
        public AggregateVariationDto Variations { get; set; } = new AggregateVariationDto();
        public List<AggregateByPeriodDto> AggregatesVariationsByMonth { get; set; } = new List<AggregateByPeriodDto>();
    } 
}
