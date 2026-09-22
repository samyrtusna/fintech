using fintech.API.Application.DTOs.ExchangeRateApiDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;

namespace fintech.API.Infrastructure.Services
{
    public class ExchangeRateApiService : IExchangeRateApiService
    {
        private string? _apiKey;
        private readonly string? apiUrl;
        private readonly HttpClient client; 
        private readonly IOptionsRepository _optionsRepository;
        private readonly IEncryptionService _encryptionService;
        public ExchangeRateApiService(IConfiguration configuration,
                                      IEncryptionService encryptionService,
                                      IHttpClientFactory httpClientFactory,
                                      IOptionsRepository optionsRepository)
        {
            apiUrl = configuration["ExchangeRateAPI:BaseUrl"];
            client = httpClientFactory.CreateClient();
            _optionsRepository = optionsRepository;
            _encryptionService = encryptionService;
        }
        public async Task<Dictionary<string, string>> GetSymbolsAsync()
        {
            string apiKey = await GetApiKeyAsync()?? throw new NotFoundException("Exchange rate API Decrypted key not found.");
            string url = $"{apiUrl}/symbols?access_key={Uri.EscapeDataString(apiKey)}";

            HttpResponseMessage response = await client.GetAsync(url);

            //TODO: add cache to cache the symbols
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExchangeRateSymbolsResponseDto>();

                if(result is null || !result.Success)
                {
                    throw new Exception("API request was successful but returned success = false.");
                }
                return result.Symbols;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Failed to retrieve symbols. " +
                    $"Status code: {response.StatusCode}. " +
                    $"Response: {errorBody}");
            }
        }

        public async Task<decimal> GetEchangeRateAsync(string transactionCurrency, string baseCurrency)
        {
            string apiKey = await GetApiKeyAsync();
            string url = $"{apiUrl}/latest?access_key={Uri.EscapeDataString(apiKey)}&base={transactionCurrency}&symbols={baseCurrency}";

            HttpResponseMessage response = await client.GetAsync(url); 

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExchangeRateResponseDto>();

                if (result != null && result.Rates.TryGetValue(baseCurrency, out decimal rate))
                {
                    return rate;
                }
                throw new Exception($"Rate for '{baseCurrency}' was not found in the API response.");
            }
            string errorDetails = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to retrieve exchange rate ({response.StatusCode}): {errorDetails}");
        }

        public async Task<string> StoreEncryptedApiKey(string apiKey)
        {
            string encryptedApiKey = _encryptionService.Encrypt(apiKey);
            var newApiKeyEntity = new Options { Key = "ExchangeRateApiKey", Value = encryptedApiKey };
            await _optionsRepository.AddAsync(newApiKeyEntity);
            await _optionsRepository.SaveChangesAsync();

            return encryptedApiKey;
        }

        private async Task<string> GetApiKeyAsync()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                var apiKeyEntity = await _optionsRepository.GetByKey("ExchangeRateApiKey") ?? throw new NotFoundException("Exchange rate API key not found.");
                var EncryptedApiKey = apiKeyEntity.Value;
                _apiKey = _encryptionService.Decrypt(EncryptedApiKey);
            }
            return _apiKey;
        }
    }
}