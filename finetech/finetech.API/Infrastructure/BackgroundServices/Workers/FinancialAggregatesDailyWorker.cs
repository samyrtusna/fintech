using fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices;

namespace fintech.API.Infrastructure.BackgroundServices.Workers
{
    public class FinancialAggregatesDailyWorker(IServiceScopeFactory scopeFactory,
        ILogger<FinancialAggregatesDailyWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var nextRun = now.Date.AddDays(1);
                    var delay = nextRun - now;

                    if(delay > TimeSpan.Zero)
                    await Task.Delay(delay, stoppingToken);

                    var targetDate = DateTime.UtcNow.Date.AddDays(-1);

                    await using var scope = scopeFactory.CreateAsyncScope();
                    var service = scope.ServiceProvider.GetRequiredService<IDailyFinancialAggregatesService>();

                    await service.CreateDailyFinancialAggregates(targetDate, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in FinancialCalculationsDailyWorker: {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
