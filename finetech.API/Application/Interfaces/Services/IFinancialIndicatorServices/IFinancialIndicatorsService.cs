using fintech.API.Application.DTOs.FinancialIndicatorsDtos;
using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;

namespace fintech.API.Application.Interfaces.Services.IFinancialIndicatorServices
{
    public interface IFinancialIndicatorsService
    {
        FinancialIndicatorsResponseDto ComputeFinancialIndicators(FinancialIntelligenceSnapshotResponseDto dto);
        Task CreateFinancialIndicators(FinancialIntelligenceSnapshotResponseDto dto, CancellationToken cancellationToken); 
    }
}
