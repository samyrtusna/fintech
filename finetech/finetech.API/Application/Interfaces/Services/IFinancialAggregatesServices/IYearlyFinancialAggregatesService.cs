namespace fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices
{
    public interface IYearlyFinancialAggregatesService
    {
        Task CreateYearlyFinancialAggregates(DateTime date, CancellationToken cancellationToken = default);
    }
}
