using AutoMapper;
using fintech.Application.DTOs.TransactionDtos;
using fintech.Application.Exceptions;
using fintech.Application.Interfaces.Repositories;
using fintech.Application.Interfaces.Services.ITransactionServices;
using fintech.Domain.Entities;


namespace fintech.Application.Services.TransactionServices
{
    public class TransactionCommandService(ITransactionRepository transactionRepository, 
        ICategoryRepository categoryRepository,
        IUserCategorySettingRepository categorySettingRepository,
        IMapper mapper) : ITransactionCommandService
    {
        public async Task<TransactionResponseDto> CreateTransactionAsync(TransactionRequestDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var category = await categoryRepository.GetAsync(dto.CategoryId) ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            if(!category.IsSystem && category.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to access this category.");
            }
            var categorySetting = await categorySettingRepository.GetByIdAsync(dto.CategoryId); 

            var newTransaction = mapper.Map<FinancialTransaction>(dto);

            newTransaction.UserId = userId;
            newTransaction.BaseAmount = newTransaction.Amount * newTransaction.ExchangeRate;
            newTransaction.IsEssential = dto.IsEssential ?? categorySetting?.IsEssential ?? false;

            await transactionRepository.AddAsync(newTransaction);
            await transactionRepository.SaveChangesAsync();

            return mapper.Map<TransactionResponseDto>(newTransaction);
        }


        public async Task<TransactionResponseDto> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequestDto dto, Guid userId)
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

            return mapper.Map<TransactionResponseDto>(transaction);
        }

        public async Task DeleteTransactionAsync(Guid transactionId, Guid userId)
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
