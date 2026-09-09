using System.Text.Json.Serialization;

namespace fintech.API.Application.DTOs.ExchangeRateApiDtos
{
    public class ExchangeRateResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = [];
    }
}
