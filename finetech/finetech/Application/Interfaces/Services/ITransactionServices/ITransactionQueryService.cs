using fintech.Application.DTOs.TransactionDtos;

namespace fintech.Application.Interfaces.Services.ITransactionServices
{
    public interface ITransactionQueryService
    {
        Task<TransactionResponseDto> GetTransactionByIdAsync(Guid transactionId, Guid userId);
        Task<IEnumerable<TransactionResponseDto>> GetTransactionsByFilterAsync(TransactionFilterDto dto, Guid userId); 
    }
}
