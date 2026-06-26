using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Interfaces.Services.IFinancialIntelligenceSnapshotServices
{
    public interface IFinancialIntelligenceSnapshotService
    {
        Task<FinancialIntelligenceSnapshotRequestDto> ComputeDailyFinancialIntelligenceSnapshot(Guid UserId, DateTime date, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshotRequestDto> ComputeMonthlyFinancialIntelligenceSnapshot(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshotRequestDto> ComputeYearlyFinancialIntelligenceSnapshot(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<FinancialIntelligenceSnapshotResponseDto> CreateFinancialIntelligenceSnapshot(FinancialIntelligenceSnapshotRequestDto dto, CancellationToken cancellationToken = default);
    }
}
