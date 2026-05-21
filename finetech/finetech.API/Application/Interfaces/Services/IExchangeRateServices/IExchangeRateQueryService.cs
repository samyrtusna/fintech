using fintech.API.Application.DTOs.ExchangeRateDtos;

namespace fintech.API.Application.Interfaces.Services.IExchangeRateServices
{
    public interface IExchangeRateQueryService
    {
        Task<ExchangeRateResponseDto?> GetExchangeRateByIdAsync(Guid id);
    }
}
