using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Services
{
    public class BudgetService(ICategoryRepository categoryRepository,
        IFinancialTransactionRepository financialTransactionRepository,
        ICurrencyConversions currencyConversions,
        IBudgetRepository budgetRepository) : IBudgetService
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
            var existingBudget = await budgetRepository.GetByPeriodAsync(userId, dto.CategoryId!.Value, dto.Period);
            if (existingBudget != null)
            {
                throw new BadRequestException("A budget for the specified Category in the specified period already exists.");
            }
            if(dto.Type == BudgetType.ByCategory)
            {
                var monthBudgets = await budgetRepository.GetAllByMonthAsync(userId, dto.Period);
                var categoryBudgetExists = monthBudgets.Any(b => b.Type == BudgetType.ByCategory);
                var globalBudget = monthBudgets.FirstOrDefault(b => b.Type == BudgetType.Global);

                if(globalBudget is not null && !categoryBudgetExists)
                {
                    globalBudget.Amount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);
                    budgetRepository.Update(globalBudget);
                }else if(globalBudget is not null && categoryBudgetExists)
                {
                    var dtoConvertedAmount= await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);
                    var convertedGlobalBudget = await currencyConversions.ConvertBudgetToBaseCurrencyAsync(userId, [globalBudget]);
                    
                    var budgetAmount = convertedGlobalBudget[0].Amount.Amount;
                    var newAmount = budgetAmount + dtoConvertedAmount.Amount;
                    var newCurrency = convertedGlobalBudget[0].Amount.Currency;
                    var amount = new Money(newAmount, newCurrency);
                    globalBudget.Amount = amount;
                    budgetRepository.Update(globalBudget);
                }
                else
                {
                    var dtoConvertedAmount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);

                    var newBudget = new Budget
                    {
                        UserId = userId,
                        Type = BudgetType.Global,
                        Amount = dtoConvertedAmount,
                        Period = new Period(
                        dto.Period.StartDate,
                        dto.Period.EndDate
                    )
                    };
                    await budgetRepository.AddAsync(newBudget);
                }

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
            var fetchedBudgets = await budgetRepository.GetAllByMonthAsync(userId, period);
            var convertedBudgets = await currencyConversions.ConvertBudgetToBaseCurrencyAsync(userId, fetchedBudgets);
            var budgets = new List<BudgetResponseDto>();
            foreach (var budget in convertedBudgets)
            {
                if(budget.Type == BudgetType.ByCategory)
                {
                    var categoryTransactions = await financialTransactionRepository.GetTotalCategoryByMonthAsync(userId, budget.Category!.Name, month);
                    var ConvertedCategoryTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, categoryTransactions);

                    var BudgetConsumption = ConvertedCategoryTransactions.Sum(t => t.Amount.Amount);
                    var BudgetConsumptionPercentage = budget.Amount.Amount > 0 ? (BudgetConsumption / budget.Amount.Amount) * 100 : 0;

                    var budgetResponseDto = budget.MapToResponseDto();
                    budgetResponseDto.BudgetConsumption = BudgetConsumption;
                    budgetResponseDto.BudgetConsumptionPercentage = BudgetConsumptionPercentage;
                    budgets.Add(budgetResponseDto);
                }else
                {
                    var expenseTransactions = await financialTransactionRepository.GetByTypeByMonthAsync(userId, FinancialType.Expense, month);
                    var convertedExpenseTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, expenseTransactions);
                    var totalExpenses = convertedExpenseTransactions.Sum(t => t.Amount.Amount);
                    var BudgetConsumptionPercentage = budget.Amount.Amount > 0 ? (totalExpenses / budget.Amount.Amount) * 100 : 0;

                    var budgetResponseDto = budget.MapToResponseDto();
                    budgetResponseDto.BudgetConsumption = totalExpenses;
                    budgetResponseDto.BudgetConsumptionPercentage = BudgetConsumptionPercentage;
                    budgets.Add(budgetResponseDto);
                }
                
            }
            return budgets;
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
            var convertedBudget = await currencyConversions.ConvertBudgetToBaseCurrencyAsync(userId, [budget]);
            var categoryTransactions = await financialTransactionRepository.GetTotalCategoryByMonthAsync(userId, budget.Category!.Name, budget.Period.StartDate.ToDateTime(TimeOnly.MinValue));
            var ConvertedCategoryTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, categoryTransactions);

            var BudgetConsumption = ConvertedCategoryTransactions.Sum(t => t.Amount.Amount);
            var BudgetConsumptionPercentage = convertedBudget[0].Amount.Amount > 0 ? (BudgetConsumption / convertedBudget[0].Amount.Amount) * 100 : 0;

            var budgetResponseDto = budget.MapToResponseDto();
            budgetResponseDto.BudgetConsumption = BudgetConsumption;
            budgetResponseDto.BudgetConsumptionPercentage = BudgetConsumptionPercentage;

            return budgetResponseDto;
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
            ;
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
                throw new UnauthorizedException($"Budget with ID {id} has already been deleted.");
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
