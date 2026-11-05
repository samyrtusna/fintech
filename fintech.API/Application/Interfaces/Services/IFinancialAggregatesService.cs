using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.FinancialAggregatesDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialAggregatesService
    {
        Task<ConfirmationResponseDto> CreateMonthlyAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken);
        Task<FinancialAggregatesResponseDto> GetMonthlyAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken);
        Task<List<FinancialAggregateDto>> GetGlobalAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken);
    }
}
