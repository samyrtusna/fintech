using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;
using Microsoft.EntityFrameworkCore;



namespace fintech.API.Application.Services 
{
    public class FinancialTransactionService(IFinancialTransactionRepository transactionRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IUserCategorySettingRepository categorySettingRepository,
        IExchangeRateApiService exchangeRateApiService) : IFinancialTransactionService 
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

            var newTransaction = dto.MapToEntity();
            if (string.Equals(dto.Currency, user.BaseCurrency, StringComparison.OrdinalIgnoreCase))
            {
                newTransaction.ExchangeRate = 1;
            }
            else
            {
                newTransaction.ExchangeRate = await exchangeRateApiService.GetEchangeRateAsync(dto.Currency, user.BaseCurrency);
            }

            newTransaction.UserId = userId; 
            newTransaction.Type = category.Type;
            newTransaction.BaseAmount = newTransaction.Amount * newTransaction.ExchangeRate;
            newTransaction.IsEssential = dto.IsEssential ?? categorySetting?.IsEssential ?? false;
            
            await transactionRepository.AddAsync(newTransaction);
            await transactionRepository.SaveChangesAsync();

            return newTransaction.MapToResponseDto();  
        }

        public async Task<FinancialTransactionResponseDto> GetFinancialTransactionByIdAsync(Guid transactionId, Guid userId)
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
            return transaction.MapToResponseDto();
        }

        public async Task<PaginatedResult<FinancialTransactionResponseDto>> GetFinancialTransactionsByFilterAsync(FinancialTransactionFilterDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (dto.Month.HasValue && (dto.Month < 1 || dto.Month > 12))
            {
                throw new BadRequestException("Month must be between 1 and 12.");
            }
            if (dto.Month.HasValue && !dto.Year.HasValue)
            {
                throw new BadRequestException("Month filter requires year.");
            }
            if (dto.Year.HasValue && (dto.Year < 2026 || dto.Year > DateTime.Now.Year))
            {
                throw new BadRequestException("Year must be between 2026 and current year.");
            }
            var query = transactionRepository.Query().Where(t => t.UserId == userId && !t.IsDeleted);

            if (dto.Year.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Year == dto.Year.Value);
            }
            if (dto.Month.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Month == dto.Month.Value);
            }
            if(dto.Day.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Day == dto.Day.Value);
            }  
            if (dto.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == dto.CategoryId.Value);
            }
            if (dto.IsEssential.HasValue)
            {
                query = query.Where(t => t.IsEssential == dto.IsEssential.Value);
            }
            var totalCount = await query.CountAsync();
            var transactions = await query
                .Include(t => t.Category)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return new PaginatedResult<FinancialTransactionResponseDto>
            {
                Items = transactions.Select(t => t.MapToResponseDto()),
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
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
            dto.UpdateEntity(transaction);

            await transactionRepository.SaveChangesAsync();

            return transaction.MapToResponseDto();
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
