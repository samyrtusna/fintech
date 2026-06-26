namespace fintech.API.Application.Interfaces.Services.IFinancialAggregatesServices
{
    public interface IMonthlyFinancialAggregatesService
    {
        Task CreateMonthlyFinancialAggregates(DateTime date, CancellationToken cancellationToken = default);
    }
}
 