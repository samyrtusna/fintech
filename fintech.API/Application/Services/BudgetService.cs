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
        IBudgetRepository budgetRepository,
        IUserRepository userRepository) : IBudgetService
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
            var newBudget = new Budget { };
            var user = await userRepository.GetUserByIdAsync(userId);
            var userBaseCurrency = user!.UserCurrencies.FirstOrDefault(c => c.IsDefault);
            var month = new DateTime(dto.Period.StartDate.Year, dto.Period.StartDate.Month, 1);
         
            var existingBudget = await budgetRepository.GetByPeriodAsync(userId, dto.CategoryId!.Value, dto.Period);
            if (existingBudget != null)
            {
                throw new BadRequestException("A budget for the specified Category in the specified period already exists.");
            }
            if(dto.Type == BudgetType.ByCategory)
            {
                var monthBudgets = await budgetRepository.GetAllByMonthAsync(userId, month);
                var categoryBudgetExists = monthBudgets.Any(b => b.Type == BudgetType.ByCategory);
                var globalBudget = monthBudgets.FirstOrDefault(b => b.Type == BudgetType.Global);

                var dtoConvertedAmount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);
                

                var categoryTransactions = await financialTransactionRepository.GetTotalCategoryByMonthAsync(userId, dto.CategoryId!.Value, month);
                var convertedCategoryTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, categoryTransactions);

                var budgetConsumption = convertedCategoryTransactions.Sum(t => t.Amount.Amount);

                if (globalBudget is not null && !categoryBudgetExists)
                {
                    globalBudget.Amount = dtoConvertedAmount;
                    globalBudget.BudgetConsumption = budgetConsumption;
                    
                    budgetRepository.Update(globalBudget);

                }else if(globalBudget is not null && categoryBudgetExists)
                {
                    var convertedGlobalBudget = await currencyConversions.ConvertToBaseCurrencyAsync(userId, globalBudget!.Amount);

                    var newAmount = convertedGlobalBudget.Amount + dtoConvertedAmount.Amount;
                    var newCurrency = convertedGlobalBudget.Currency;
                    var amount = new Money(newAmount, newCurrency);
                    globalBudget.Amount = amount;
                    globalBudget.BudgetConsumption += budgetConsumption;
                    budgetRepository.Update(globalBudget);

                    if (monthBudgets[0].Amount.Currency!.Code != userBaseCurrency!.Currency.Code)
                    {
                        foreach (var budget in monthBudgets)
                        {
                            if (budget.Type == BudgetType.ByCategory)
                            {
                                budget.Amount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, budget.Amount);
                                budgetRepository.Update(budget);
                            }
                        }
                    }
                }
                else
                {
                    var ConvertedDtoAmount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);

                    var newGlobalBudget = new Budget
                    {
                        UserId = userId,
                        Type = BudgetType.Global,
                        Amount = ConvertedDtoAmount,
                        BudgetConsumption = budgetConsumption,
                        Period = new Period(
                        dto.Period.StartDate,
                        dto.Period.EndDate
                    )
                    };
                    
                    await budgetRepository.AddAsync(newGlobalBudget); 
                }
                newBudget = dto.MapToEntity(userId);
                newBudget.Amount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, dto.Amount);
                newBudget.BudgetConsumption = budgetConsumption;
                await budgetRepository.AddAsync(newBudget);
                await budgetRepository.SaveChangesAsync();
            }
            //TODO: add Global budget creation logic.
            return newBudget.MapToResponseDto();
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
            var fetchedBudgets = await budgetRepository.GetAllByMonthAsync(userId, month);
            
            var budgets = new List<BudgetResponseDto>();
            foreach (var budget in fetchedBudgets)
            {
                var BudgetConsumptionPercentage = budget.Amount.Amount > 0 ? (budget.BudgetConsumption / budget.Amount.Amount) * 100 : 0;

                var budgetResponseDto = budget.MapToResponseDto();
                budgetResponseDto.BudgetConsumptionPercentage = BudgetConsumptionPercentage;
                budgets.Add(budgetResponseDto);                
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

            var BudgetConsumptionPercentage = budget.Amount.Amount > 0 ? (budget.BudgetConsumption / budget.Amount.Amount) * 100 : 0;

            var budgetResponseDto = budget.MapToResponseDto();
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
            var month = new DateTime(budget.Period.StartDate.Year, budget.Period.StartDate.Month, 1);
            var monthBudgets = await budgetRepository.GetAllByMonthAsync(userId, month);
            var categoryBudgetExists = monthBudgets.Any(b => b.Type == BudgetType.ByCategory);
            if(budget.Type == BudgetType.Global && categoryBudgetExists)
            {
                throw new BadRequestException("Cannot update global budget in this case");
            }
            var convertedAmount = await currencyConversions.ConvertToBaseCurrencyAsync(userId, amount);

            if (budget.Type == BudgetType.ByCategory)
            {
                var globalBudget = monthBudgets.FirstOrDefault(b => b.Type == BudgetType.Global);
                var newGlobalAmount = globalBudget!.Amount.Amount - budget.Amount.Amount + convertedAmount.Amount;
                var currency = new Currency(globalBudget.Amount.Currency!.Code, globalBudget.Amount.Currency.Symbol);
                globalBudget.Amount = new Money(newGlobalAmount, currency);
                budgetRepository.Update(globalBudget);
            }

            budget.Amount = convertedAmount;
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
            var month = new DateTime(budget.Period.StartDate.Year, budget.Period.StartDate.Month, 1);
            var monthBudgets = await budgetRepository.GetAllByMonthAsync(userId, month);
            var budgetsByCategory = monthBudgets.Where(b => b.Type == BudgetType.ByCategory).ToList();
            if (budget.Type == BudgetType.Global && budgetsByCategory.Count > 0) 
            {
                throw new BadRequestException("Cannot delete global budget in this case");
            }

            if (budget.Type == BudgetType.ByCategory)
            {
                var globalBudget = monthBudgets.FirstOrDefault(b => b.Type == BudgetType.Global);
                if(budgetsByCategory.Count == 1)
                {
                    globalBudget!.IsDeleted = true;
                    budgetRepository.Update(globalBudget);
                }
                else
                {
                    var newGlobalAmount = globalBudget!.Amount.Amount - budget.Amount.Amount;
                    var currency = new Currency(globalBudget.Amount.Currency!.Code, globalBudget.Amount.Currency.Symbol);
                    globalBudget.Amount = new Money(newGlobalAmount, currency);
                    globalBudget.BudgetConsumption -= budget.BudgetConsumption;
                    budgetRepository.Update(globalBudget);
                }               
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
