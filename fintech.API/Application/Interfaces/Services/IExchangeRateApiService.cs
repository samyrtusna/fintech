namespace fintech.API.Application.Interfaces.Services
{
    public interface IExchangeRateApiService
    {
        Task<Dictionary<string, string>> GetSymbolsAsync();
        Task<decimal> GetEchangeRateAsync(string transactionCurrency, string baseCurrency);
        Task<string> StoreEncryptedApiKey(string apiKey);
    }
}
