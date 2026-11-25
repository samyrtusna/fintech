using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.GoalDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IGoalService
    {
        Task<GoalResponseDto> CreateGoalsAsync(Guid userId, GoalRequestDto dto);
        Task<List<GoalResponseDto>> GetGoalsByMonthAsync(Guid userId, DateTime month);
        Task<GoalResponseDto> GetGoalByIdAsync(Guid userId, Guid goalId);
        Task<GoalResponseDto> UpdateGoalAsync(Guid userId, Guid goalId, GoalUpdateDto dto);
        Task<ConfirmationResponseDto> DeleteGoalAsync(Guid userId, Guid goalId);
    }
}
