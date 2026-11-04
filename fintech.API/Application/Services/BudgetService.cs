using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Services
{
    public class BudgetService(ICategoryRepository categoryRepository, IBudgetRepository budgetRepository) : IBudgetService
    {
        public async Task<BudgetResponseDto> CreateNewBudgetAsync(Guid userId, BudgetRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if(dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty)
            {
                var category = await categoryRepository.GetAsync((Guid)dto.CategoryId) ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
                if (!category.IsSystem && category.UserId != userId)
                {
                    throw new UnauthorizedException("You do not have permission to access this category.");
                }
                if(category.Type != FinancialType.Expense)
                {
                    throw new BadRequestException("Budgets can only be created for expense categories.");
                }
            }
            //TODO: Add other business rules for budget creation.
            var existingBudget = await budgetRepository.GetByPeriodAsync(userId, dto.Period);
            if (existingBudget != null)
            {
                throw new BadRequestException("A budget for the specified Category in the specified period already exists.");
            }
            var budget = dto.MapToEntity(userId);
            await budgetRepository.AddAsync(budget);
            await budgetRepository.SaveChangesAsync();

            return budget.MapToResponseDto();
        }

        public async Task<List<BudgetResponseDto>> GetBudgetsByMonthAsync(Guid userId, DateTime month)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var startDate = new DateOnly(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var period = new Period(startDate, endDate);
            var budgets = await budgetRepository.GetAllByMonthAsync(userId, period);
            return [.. budgets.Select(b => b.MapToResponseDto())];
        }

        public async Task<BudgetResponseDto> GetBudgetByIdAsync(Guid userId, Guid id)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Budget ID cannot be empty.", nameof(id));
            }
            var budget = await budgetRepository.GetBudgetByIdAsync(id) ?? throw new NotFoundException($"Budget with ID {id} not found.");
            
            if(budget.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this budget.");
            }
            if (budget.IsDeleted)
            {
                throw new NotFoundException($"Budget with ID {id} has been deleted.");
            }
            if(!budget.IsActive)
            {
                throw new BadRequestException($"Budget with ID {id} is not active.");
            }

            return budget.MapToResponseDto();
        }

        public async Task<BudgetResponseDto> UpdateBudgetAsync(Guid userId, Guid id, Money amount)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Budget ID cannot be empty.", nameof(id));
            }
            var budget = await budgetRepository.GetBudgetByIdAsync(id) ?? throw new NotFoundException($"Budget with ID {id} not found.");

            if (budget.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to update this budget.");
            }
            if (budget.IsDeleted)
            {
                throw new NotFoundException($"Budget with ID {id} has been deleted.");
            }
            if (!budget.IsActive)
            {
                throw new BadRequestException($"Budget with ID {id} is not active.");
            }

            budget.Amount = amount;
            budgetRepository.Update(budget);
            await budgetRepository.SaveChangesAsync();
            return budget.MapToResponseDto();
        }

        public async Task<ConfirmationResponseDto> DeleteBudgetAsync(Guid userId, Guid id)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Budget ID cannot be empty.", nameof(id));
            }
            var budget = await budgetRepository.GetBudgetByIdAsync(id) ?? throw new NotFoundException($"Budget with ID {id} not found.");
            if (budget.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to delete this budget.");
            }
            if (budget.IsDeleted)
            {
                throw new NotFoundException($"Budget with ID {id} has already been deleted.");
            }
            budget.IsDeleted = true;
            budgetRepository.Update(budget);
            await budgetRepository.SaveChangesAsync();
            return new ConfirmationResponseDto
            {
                Message = $"Budget with ID {id} has been successfully deleted."
            };
        }
    }
}
