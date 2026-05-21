using AutoMapper;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Exceptions;
using fintech.API.Domain.Entities;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Interfaces.Services.IFinancialTransactionServices;


namespace fintech.API.Application.Services.FinancialTransactionServices 
{
    public class FinancialTransactionCommandService(IFinancialTransactionRepository transactionRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IUserCategorySettingRepository categorySettingRepository,
        IExchangeRateRepository exchangeRateRepository,
        IMapper mapper) : IFinancialTransactionCommandService
    {
        public async Task<FinancialTransactionResponseDto> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto); 
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));  
            }
            var user = await userRepository.GetByIdAsync(userId) ?? throw new NotFoundException($"User with ID {userId} not found.");
            var category = await categoryRepository.GetAsync(dto.CategoryId) ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            if(!category.IsSystem && category.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this category.");
            }
            var categorySetting = await categorySettingRepository.GetByIdAsync(dto.CategoryId); 

            var newTransaction = mapper.Map<FinancialTransaction>(dto);
            if (string.Equals(dto.Currency, user.BaseCurrency, StringComparison.OrdinalIgnoreCase))
            {
                newTransaction.ExchangeRate = 1;
            }
            else
            {
                var exchangeRate = await exchangeRateRepository.GetLatestAsync(dto.Currency, user.BaseCurrency) ?? throw new NotFoundException($"Exchange rate for currency {dto.Currency} not found.");
                newTransaction.ExchangeRate = exchangeRate.Rate;
            }
            newTransaction.UserId = userId;
            newTransaction.Type = category.Type;
            newTransaction.BaseAmount = newTransaction.Amount * newTransaction.ExchangeRate;
            newTransaction.IsEssential = dto.IsEssential ?? categorySetting?.IsEssential ?? false;

            await transactionRepository.AddAsync(newTransaction);
            await transactionRepository.SaveChangesAsync();

            return mapper.Map<FinancialTransactionResponseDto>(newTransaction);  
        }


        public async Task<FinancialTransactionResponseDto> UpdateFinancialTransactionAsync(Guid transactionId, UpdateFinancialTransactionRequestDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (transactionId == Guid.Empty) 
            {
                throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var transaction = await transactionRepository.GetAsync(transactionId) ?? throw new NotFoundException($"Transaction with ID {transactionId} not found.");
            if (transaction.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this transaction.");
            }
            mapper.Map(dto, transaction);

            await transactionRepository.SaveChangesAsync();

            return mapper.Map<FinancialTransactionResponseDto>(transaction);
        }

        public async Task DeleteFinancialTransactionAsync(Guid transactionId, Guid userId)
        {
            if (transactionId == Guid.Empty)
            { 
                throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var transaction = await transactionRepository.GetAsync(transactionId) ?? throw new NotFoundException($"Transaction with ID {transactionId} not found.");
            if (transaction.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this transaction.");
            }
            transaction.IsDeleted = true;

            await transactionRepository.SaveChangesAsync();
        }
    }
}
