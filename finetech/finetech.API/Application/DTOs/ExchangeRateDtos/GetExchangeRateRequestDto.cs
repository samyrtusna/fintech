namespace fintech.API.Application.DTOs.ExchangeRateDtos
{
    public class GetExchangeRateRequestDto
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
    }
}
