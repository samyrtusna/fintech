using AutoMapper;
using fintech.API.Application.DTOs.ExchangeRateDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services.IExchangeRateServices;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Services.ExchangeRateServices
{
    public class ExchangeRateCommandService(IExchangeRateRepository repository, IMapper mapper) : IExchangeRateCommandService
    {
        public async Task<ExchangeRateResponseDto> CreateExchangeRateAsync(CreateExchangeRateRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var exchangeRate = mapper.Map<ExchangeRate>(dto);
            
            await repository.AddAsync(exchangeRate);
            await repository.SaveChangesAsync();

            return mapper.Map<ExchangeRateResponseDto>(exchangeRate);
        }
    }
}
  