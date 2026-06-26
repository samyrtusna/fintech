namespace fintech.API.Application.DTOs.ExchangeRateDtos
{
    public class ExchangeRateResponseDto
    {
        public Guid Id { get; set; }
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
