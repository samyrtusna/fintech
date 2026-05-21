using fintech.API.Application.DTOs.FinancialTransactionDtos;

namespace fintech.API.Application.Interfaces.Services.IFinancialTransactionServices 
{
    public interface IFinancialTransactionQueryService
    {
        Task<FinancialTransactionResponseDto> GetFinancialTransactionByIdAsync(Guid transactionId, Guid userId);
        Task<IEnumerable<FinancialTransactionResponseDto>> GetFinancialTransactionsByFilterAsync(FinancialTransactionFilterDto dto, Guid userId); 
    }
}
 