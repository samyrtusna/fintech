using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Persistence;
using fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices;
using fintech.API.Application.Interfaces.Services.IFinancialIndicatorServices;
using fintech.API.Application.Interfaces.Services.IFinancialIntelligenceSnapshotServices;

namespace fintech.API.Application.Services.FinancialAggregatesServices
{
    public class YearlyFinancialAggregatesService(IUnitOfWork unitOfWork,
        IFinancialIntelligenceSnapshotService financialIntelligenceSnapshotService,
        IFinancialIndicatorsService financialIndicatorsService, ILogger<YearlyFinancialAggregatesService> logger) : IYearlyFinancialAggregatesService
    {
        public async Task CreateYearlyFinancialAggregates(DateTime date, CancellationToken cancellationToken = default)
        {
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("Date cannot be empty", nameof(date));
            }
            var users = await unitOfWork.Users.GetAllAsync(cancellationToken) ?? throw new NotFoundException("users not found");
            foreach (var user in users)
            {
                var existingSnapshot = await unitOfWork.FinancialIntelligenceSnapshots.GetByYearAsync(user.Id, date, cancellationToken);
                if (existingSnapshot != null)
                {
                    logger.LogInformation("Yearly financial intelligence snapshot already exists for user {UserId} on {Date}, skipping", user.Id, date);
                    continue;
                }
                try
                {
                    var snapshotDto = await financialIntelligenceSnapshotService.ComputeYearlyFinancialIntelligenceSnapshot(user.Id, date, cancellationToken);
                    var snapshot = await financialIntelligenceSnapshotService.CreateFinancialIntelligenceSnapshot(snapshotDto, cancellationToken);
                    await financialIndicatorsService.CreateFinancialIndicators(snapshot, cancellationToken);

                    await unitOfWork.SaveAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process financial aggregates for user {UserId} on {Date}", user.Id, date);
                }
            }
        }
    }
}
