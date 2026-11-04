namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialAggregatesResponseDto
    {
        public List<FinancialAggregateDto> Aggregates { get; set; } = [];
        public List<AggregateVariationsDto> PeriodicVariations { get; set; } = [];
    }
}
 