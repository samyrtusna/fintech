using System.Text.Json.Serialization;

namespace fintech.API.Application.DTOs.ExchangeRateApiDtos
{
    public class ExchangeRateSymbolsResponseDto
    {
        [JsonPropertyName("iso_code")]
        public string IsoCode { get; set; } = string.Empty;

        [JsonPropertyName("iso_numeric")]
        public string IsoNumeric { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;
    }
}
