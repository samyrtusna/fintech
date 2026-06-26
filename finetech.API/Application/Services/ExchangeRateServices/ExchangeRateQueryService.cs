using AutoMapper;
using fintech.API.Application.DTOs.ExchangeRateDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services.IExchangeRateServices;
using fintech.API.Application.Exceptions;

namespace fintech.API.Application.Services.ExchangeRateServices
{
    public class ExchangeRateQueryService(IExchangeRateRepository repository, IMapper mapper) : IExchangeRateQueryService
    {
        
        public async Task<ExchangeRateResponseDto?> GetExchangeRateByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            var exchangeRate = await repository.GetByIdAsync(id) ?? throw new NotFoundException($"Exchange rate with id {id} not found.");

            return mapper.Map<ExchangeRateResponseDto>(exchangeRate);
        }  
    }
}
