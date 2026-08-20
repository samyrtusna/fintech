using System.Text.Json.Serialization;

namespace fintech.API.Application.DTOs.ExchangeRateApiDtos
{
    public class ExchangeRateSymbolsResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("symbols")]
        public Dictionary<string, string> Symbols { get; set; } = [];
    }
}
