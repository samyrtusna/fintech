using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IBudgetService
    {
        Task<BudgetResponseDto> CreateNewBudgetAsync(Guid userId, BudgetRequestDto dto);
        Task<List<BudgetResponseDto>> GetBudgetsByMonthAsync(Guid userId, DateTime month);
        Task<BudgetResponseDto> GetBudgetByIdAsync(Guid userId, Guid id);
        Task<BudgetResponseDto> UpdateBudgetAsync(Guid userId, Guid id, Money amount);
        Task<ConfirmationResponseDto> DeleteBudgetAsync(Guid userId, Guid id);
    }
}
