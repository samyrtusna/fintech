using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.GoalDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Services
{
    public class GoalService(ICategoryRepository categoryRepository, IGoalRepository goalRepository) : IGoalService
    {
        public async Task<GoalResponseDto> CreateGoalsAsync(Guid userId, GoalRequestDto dto )
        {
            ArgumentNullException.ThrowIfNull(dto,nameof(dto));
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var goals = await goalRepository.GetAllByMonthAsync(userId, dto.Period);
            if (dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty)
            {
                var category = await categoryRepository.GetAsync((Guid)dto.CategoryId) ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
                var existingGoal = goals.FirstOrDefault(g => g.CategoryId == dto.CategoryId);
                if (!category.IsSystem && category.UserId != userId)
                {
                    throw new UnauthorizedException("You do not have permission to access this category.");
                }
                if(existingGoal != null)
                {
                    throw new BadRequestException($"A goal for the category '{category.Name}' already exists for the specified period.");
                }
                if (category.Type != FinancialType.Investment && category.Type != FinancialType.Savings)
                {
                    throw new BadRequestException("Goals can only be created for investment or savings categories.");
                }
                if(dto.TargetType.HasValue && dto.TargetType.Value != category.Type)
                {
                    throw new BadRequestException($"The target type must match the category type. Expected: {category.Type}, but got: {dto.TargetType.Value}.");
                }
            }
            else if (dto.TargetType.HasValue)
            {
                var existingGoal = goals.FirstOrDefault(g => g.TargetType == dto.TargetType);
                if (existingGoal != null)
                {
                    throw new BadRequestException($"A goal for the type '{dto.TargetType.Value}' already exists for the specified period.");
                }
                if (dto.TargetType.Value != FinancialType.Investment && dto.TargetType.Value != FinancialType.Savings)
                {
                    throw new BadRequestException("Goals can only be created for investment or savings target types.");
                }
            }
            var goal = dto.ToEntity(userId);
            await goalRepository.AddAsync(goal);
            await goalRepository.SaveChangesAsync();

            return goal.ToDto();
        }

        public async Task<List<GoalResponseDto>> GetGoalsByMonthAsync(Guid userId, DateTime month)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var startDate = new DateOnly(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var period = new Period(startDate, endDate);
            var goals = await goalRepository.GetAllByMonthAsync(userId, period);
            return [..goals.Select(g => g.ToDto())];
        }

        public async Task<GoalResponseDto> GetGoalByIdAsync(Guid userId, Guid goalId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var goal = await goalRepository.GetGoalByIdAsync(goalId) ?? throw new NotFoundException($"Goal with ID {goalId} not found.");
            if (goal.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this goal.");
            }
            if (goal.IsDeleted)
            {
                throw new NotFoundException($"Goal with ID {goalId} has been deleted.");
            }
            if (!goal.IsActive)
            {
                throw new BadRequestException($"Goal with ID {goalId} is not active.");
            }
            return goal.ToDto();
        }

        public async Task<GoalResponseDto> UpdateGoalAsync(Guid userId, Guid goalId, GoalUpdateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var goal = await goalRepository.GetGoalByIdAsync(goalId) ?? throw new NotFoundException($"Goal with ID {goalId} not found.");
            if (goal.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to update this goal.");
            }
            if (goal.IsDeleted)
            {
                throw new NotFoundException($"Goal with ID {goalId} has been deleted.");
            }
            if (!goal.IsActive)
            {
                throw new BadRequestException($"Goal with ID {goalId} is not active.");
            }
            goal.Title = dto.Title ?? goal.Title;
            goal.TargetAmount = dto.TargetAmount ?? goal.TargetAmount;

            goalRepository.Update(goal);
            await goalRepository.SaveChangesAsync();
            return goal.ToDto();
        }

        public async Task<ConfirmationResponseDto> DeleteGoalAsync(Guid userId, Guid goalId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var goal = await goalRepository.GetGoalByIdAsync(goalId) ?? throw new NotFoundException($"Goal with ID {goalId} not found.");
            if (goal.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to delete this goal.");
            }
            if (goal.IsDeleted)
            {
                throw new NotFoundException($"Goal with ID {goalId} has already been deleted.");
            }
            goal.IsDeleted = true;
            goalRepository.Update(goal);
            await goalRepository.SaveChangesAsync();
            return new ConfirmationResponseDto
            {
                Message = "Goal deleted successfully.",
            };
        }
    }
}
