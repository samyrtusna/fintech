using fintech.API.Application.DTOs.FinancialTransactionDtos;

namespace fintech.API.Application.Interfaces.Services.IFinancialTransactionServices
{
    public interface IFinancialTransactionCommandService 
    {
        Task<FinancialTransactionResponseDto> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto, Guid userId);
        Task<FinancialTransactionResponseDto> UpdateFinancialTransactionAsync(Guid transactionId, UpdateFinancialTransactionRequestDto dto, Guid userId);
        Task DeleteFinancialTransactionAsync(Guid transactionId, Guid userId); 
    } 
}
