using fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices;

namespace fintech.API.Infrastructure.BackgroundServices.Workers
{
    public class FinancialAggregatesMonthlyWorker(IServiceScopeFactory scopeFactory,
        ILogger<FinancialAggregatesMonthlyWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var nextRun = now.Date.AddMonths(1).AddMinutes(5);
                    var delay = nextRun - now;

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    var targetDate = DateTime.UtcNow.Date.AddMonths(-1);

                    await using var scope = scopeFactory.CreateAsyncScope();
                    var service = scope.ServiceProvider.GetRequiredService<IMonthlyFinancialAggregatesService>(); 

                    await service.CreateMonthlyFinancialAggregates(targetDate, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in FinancialCalculationsMonthlyWorker: {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
