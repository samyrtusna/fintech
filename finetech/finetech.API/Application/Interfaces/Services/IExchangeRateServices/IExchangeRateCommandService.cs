using fintech.API.Application.DTOs.ExchangeRateDtos;

namespace fintech.API.Application.Interfaces.Services.IExchangeRateServices
{
    public interface IExchangeRateCommandService
    {
        Task<ExchangeRateResponseDto> CreateExchangeRateAsync(CreateExchangeRateRequestDto dto);
    }
}
 