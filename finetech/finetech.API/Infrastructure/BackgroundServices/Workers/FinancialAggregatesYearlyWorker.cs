using fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices;

namespace fintech.API.Infrastructure.BackgroundServices.Workers
{
    public class FinancialAggregatesYearlyWorker(IServiceScopeFactory scopeFactory,
        ILogger<FinancialAggregatesYearlyWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var nextRun = now.Date.AddYears(1).AddMinutes(15);
                    var delay = nextRun - now;

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    var targetDate = DateTime.UtcNow.Date.AddYears(-1);

                    await using var scope = scopeFactory.CreateAsyncScope();
                    var service = scope.ServiceProvider.GetRequiredService<IYearlyFinancialAggregatesService>();

                    await service.CreateYearlyFinancialAggregates(targetDate, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in FinancialCalculationsYearlyWorker: {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
