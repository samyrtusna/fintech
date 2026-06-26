using AutoMapper;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Interfaces.Services.IFinancialTransactionServices;
using fintech.API.Application.DTOs.QueryDtos;

namespace fintech.API.Application.Services.FinancialTransactionServices 
{
    public class FinancialTransactionQueryService(IFinancialTransactionRepository transactionRepository, IMapper mapper) : IFinancialTransactionQueryService
    {
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
            return mapper.Map<FinancialTransactionResponseDto>(transaction);
        }

        public async Task<PaginatedResult<FinancialTransactionResponseDto>> GetFinancialTransactionsByFilterAsync(FinancialTransactionFilterDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto);  
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if(dto.Month.HasValue && (dto.Month < 1 || dto.Month > 12))
            {
                throw new BadRequestException("Month must be between 1 and 12.");
            }
            if(dto.Month.HasValue && !dto.Year.HasValue)
            {
                throw new BadRequestException("Month filter requires year."); 
            }
            if(dto.Year.HasValue && (dto.Year < 2026 || dto.Year > DateTime.Now.Year))
            {
                throw new BadRequestException("Year must be between 2026 and current year.");
            }
            var query = transactionRepository.Query().Where(t => t.UserId == userId && !t.IsDeleted);

            if (dto.Year.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Year == dto.Year.Value);
            }
            if(dto.Month.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Month == dto.Month.Value);
            }
            if (dto.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == dto.CategoryId.Value);
            }
            if(dto.IsEssential.HasValue)
            {
                query = query.Where(t => t.IsEssential == dto.IsEssential.Value);
            }
            var totalCount = await query.CountAsync();
            var transactions = await query
                .Include(t => t.Category)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((dto.Page -1) * dto.PageSize) 
                .Take(dto.PageSize)
                .ToListAsync();

            return new PaginatedResult<FinancialTransactionResponseDto>
            {
                Items = mapper.Map<IEnumerable<FinancialTransactionResponseDto>>(transactions),
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }
    }
}
