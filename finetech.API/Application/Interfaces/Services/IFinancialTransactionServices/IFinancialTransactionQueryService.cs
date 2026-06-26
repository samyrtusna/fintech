using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;

namespace fintech.API.Application.Interfaces.Services.IFinancialTransactionServices 
{
    public interface IFinancialTransactionQueryService
    {
        Task<FinancialTransactionResponseDto> GetFinancialTransactionByIdAsync(Guid transactionId, Guid userId);
        Task<PaginatedResult<FinancialTransactionResponseDto>> GetFinancialTransactionsByFilterAsync(FinancialTransactionFilterDto dto, Guid userId); 
    }
}
 