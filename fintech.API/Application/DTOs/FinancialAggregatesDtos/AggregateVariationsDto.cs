namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class AggregateVariationsDto
    {
        public string Name { get; set; } = string.Empty;
        public List<AggregateVariationsValueByMonthDto> Values { get; set; } = [];
    } 
}
