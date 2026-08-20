using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IFinancialTransactionService  
    {
        Task<FinancialTransactionResponseDto> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto, Guid userId);
        Task<FinancialTransactionResponseDto> GetFinancialTransactionByIdAsync(Guid transactionId, Guid userId);
        Task<PaginatedResult<FinancialTransactionResponseDto>> GetFinancialTransactionsByFilterAsync(FinancialTransactionFilterDto dto, Guid userId);
        Task<FinancialTransactionResponseDto> UpdateFinancialTransactionAsync(Guid transactionId, UpdateFinancialTransactionRequestDto dto, Guid userId);
        Task DeleteFinancialTransactionAsync(Guid transactionId, Guid userId); 
    } 
}
