using AutoMapper;
using fintech.Application.DTOs.TransactionDtos;
using fintech.Application.Exceptions;
using fintech.Application.Interfaces.Repositories;
using fintech.Application.Interfaces.Services.ITransactionServices;
using Microsoft.EntityFrameworkCore;

namespace fintech.Application.Services.TransactionServices
{
    public class TransactionQueryService(ITransactionRepository transactionRepository, IMapper mapper) : ITransactionQueryService
    {
        public async Task<TransactionResponseDto> GetTransactionByIdAsync(Guid transactionId, Guid userId)
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
            return mapper.Map<TransactionResponseDto>(transaction);
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByFilterAsync(TransactionFilterDto dto, Guid userId)
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
            var transactions = await query.ToListAsync();

            return mapper.Map<IEnumerable<TransactionResponseDto>>(transactions);
        }
    }
}
