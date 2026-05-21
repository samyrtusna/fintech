namespace fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices
{
    public interface IDailyFinancialAggregatesService
    {
        Task CreateDailyFinancialAggregates(DateTime date, CancellationToken cancellationToken = default);
    }
}
