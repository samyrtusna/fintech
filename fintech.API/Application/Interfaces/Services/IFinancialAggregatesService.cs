using fintech.API.Application.DTOs.FinancialAggregatesDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialAggregatesService
    {
        Task<GlobalAggregatesResponseDto> GetGlobalAggregates(Guid userId);
        Task<FinancialAggregatesResponseDto[]> GetAggregatesByFilter(Guid userId, FinancialAggregatesFilterDto dto);
    }
}
