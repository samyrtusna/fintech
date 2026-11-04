using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class BudgetMapper
    {
        public static Budget MapToEntity(this BudgetRequestDto dto, Guid userId)
        {
            return new Budget
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                Amount = dto.Amount,
                Period = dto.Period
            };
        }

        public static BudgetResponseDto MapToResponseDto(this Budget entity)
        {
            return new BudgetResponseDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                Type = entity.Type,
                Amount = entity.Amount,
                Period = entity.Period,
                Category = entity.Category
            };
        }
    }
}
