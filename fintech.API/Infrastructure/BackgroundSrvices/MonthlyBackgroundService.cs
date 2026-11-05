using fintech.API.Application.Interfaces.Services;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.BackgroundSrvices
{
    public class MonthlyBackgroundService(IServiceProvider serviceProvider, ILogger logger) : BackgroundService
    {
        private bool _hasRunThisMonth = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                if (now.Day == 1 && !_hasRunThisMonth)
                {
                    await CreateMonthlyAggregates(stoppingToken);
                    _hasRunThisMonth = true;
                }
                else if (now.Day != 1)
                {
                    _hasRunThisMonth = false;
                }
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }


        private async Task CreateMonthlyAggregates(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var aggregatesService = scope.ServiceProvider.GetRequiredService<IFinancialAggregatesService>();

            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month - 1, 1);

            var users = await dbContext.Users.ToListAsync(stoppingToken);
           
            foreach (var user in users)
            {
                try
                {
                    await aggregatesService.CreateMonthlyAggregatesAsync(user.Id, startDate, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error creating monthly aggregates for user {user.Id}: {ex.Message}");
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
