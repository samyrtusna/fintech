using System.Text.Json.Serialization;

namespace fintech.API.Application.DTOs.ExchangeRateApiDtos
{
    public class ExchangeRateResponseDto
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("quote")]
        public string Quote { get; set; } = string.Empty;
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }
}
