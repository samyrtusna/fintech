using fintech.API.Application.DTOs.GoalDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class GoalMapper
    {
        public static Goals ToEntity(this GoalRequestDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            return new Goals
            {
                UserId = userId,
                Title = dto.Title,
                CategoryId = dto.CategoryId,
                TargetType = dto.TargetType ,
                TargetAmount = dto.TargetAmount,
                Period = dto.Period,
            };
        }

        public static GoalResponseDto ToDto(this Goals entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            return new GoalResponseDto
            {
                Id = entity.Id,
                Title = entity.Title,
                CategoryId = entity.CategoryId,
                TargetType = entity.TargetType,
                TargetAmount = entity.TargetAmount,
                Period = entity.Period,
                GoalProgress = entity.GoalProgress,
                Category = entity.Category
            };
        }
    }
}
