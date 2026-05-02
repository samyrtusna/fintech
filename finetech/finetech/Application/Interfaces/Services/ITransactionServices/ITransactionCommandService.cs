using fintech.Application.DTOs.TransactionDtos;

namespace fintech.Application.Interfaces.Services.ITransactionServices
{
    public interface ITransactionCommandService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(TransactionRequestDto dto, Guid userId);
        Task<TransactionResponseDto> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequestDto dto, Guid userId);
        Task DeleteTransactionAsync(Guid transactionId, Guid userId);
    } 
}
