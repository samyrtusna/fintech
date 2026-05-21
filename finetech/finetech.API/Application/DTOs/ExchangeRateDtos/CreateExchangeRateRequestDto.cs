namespace fintech.API.Application.DTOs.ExchangeRateDtos
{
    public class CreateExchangeRateRequestDto
    {
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
    }
}
 