using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;


namespace fintech.API.Application.Services
{
    public class FinancialAggregatesService(IFinancialAggregatesRepository financialAggregatesRepository,
        IFinancialCalculations financialCalculations,
        IFinancialTransactionRepository financialTransactionRepository,
        ICurrencyConversions currencyConversions
        ) : IFinancialAggregatesService 
    {
        public async Task<ConfirmationResponseDto> CreateMonthlyAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("A valid month must be provided.", nameof(date));
            }
            var thisMonthAggregates = await financialAggregatesRepository.GetFinancialAggregatesByMonthAsync(userId, date, cancellationToken);
            if(thisMonthAggregates.Count > 0)
            {
                throw new BadRequestException("The current month Aggregates already created.");
            }

            var monthlyAggregates = await CalculateMonthlyAggregatesAsync(userId, date); 
            var globalAggregates = await GetGlobalAggregatesAsync(userId, date, cancellationToken);

            monthlyAggregates.AddRange(globalAggregates);


            if(monthlyAggregates is not null && monthlyAggregates.Count > 0)
            {
                foreach (var aggregate in monthlyAggregates)
                {
                    var aggregateEntity = aggregate.MapToEntity(userId);

                    await financialAggregatesRepository.AddAsync(aggregateEntity, cancellationToken);
                }
                await financialAggregatesRepository.SaveChangesAsync(cancellationToken);
            }

            return new ConfirmationResponseDto { 
                Message = $"Monthly aggregate for month {date.Month}/{date.Year} succesfully created. "
            };
        }

        public async Task<FinancialAggregatesResponseDto> GetMonthlyAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("A valid month must be provided.", nameof(date));
            }
            
            var thisMonthAggregates = await financialAggregatesRepository.GetFinancialAggregatesByMonthAsync(userId, date, cancellationToken);
            var aggregatesVariations = await GetAggregateVariationsByMonthAsync(userId);

            if(thisMonthAggregates.Count > 0)
            {
                return new FinancialAggregatesResponseDto
                {
                    Aggregates = [..thisMonthAggregates.Select(ag => ag.MapToDto())],
                    PeriodicVariations = aggregatesVariations
                };
            }else
            {
                var calculatedAggregates = await CalculateMonthlyAggregatesAsync(userId, date);

                return new FinancialAggregatesResponseDto
                {
                    Aggregates = calculatedAggregates,
                    PeriodicVariations = aggregatesVariations
                };
            }
        }

        public async Task<List<FinancialAggregateDto>> GetGlobalAggregatesAsync(Guid userId, DateTime date, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("A valid month must be provided.", nameof(date));
            }

            var startDate = new DateOnly(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var period = new Period(startDate, endDate);
            var lastMonth = date.AddMonths(-1);

            var lastMonthAggregates = await financialAggregatesRepository.GetGlobalAggregatesByMonthAsync(userId, lastMonth, cancellationToken);

            var transactions = await financialTransactionRepository.GetByMonthAsync(userId, date, cancellationToken);

            var convertedTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, transactions) ?? throw new NotFoundException("Failed to convert current month's transactions to base currency");

            var lastMonthConvertedAggregates = await currencyConversions.ConvertAggregateBaseCurrencyAsync(userId, lastMonthAggregates) ?? throw new NotFoundException("Failed to convert last year's aggregates to base currency");

            var currency = new Currency(convertedTransactions.FirstOrDefault()?.BaseAmount?.Currency?.Code ?? "USD", convertedTransactions.FirstOrDefault()?.BaseAmount?.Currency?.Symbol ?? "$");

            var CurrentAggregates = CalculateAggregatesByType(convertedTransactions);




            var currentActiveDebt = CurrentAggregates.TotalContractedLoans - CurrentAggregates.TotalPrincipalRepayments;
            var currentNetCashFlow =  financialCalculations.GetNetCashFlow(CurrentAggregates.TotalIncome, CurrentAggregates.TotalExpenses, CurrentAggregates.TotalInvestments, CurrentAggregates.TotalSavings, CurrentAggregates.TotalContractedLoans, CurrentAggregates.TotalInterestPayments, CurrentAggregates.TotalPrincipalRepayments);

            var lastMonthNetCashFlow = lastMonthConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.GlobalNetCashFlow.ToString())?.Amount.Amount ?? 0;
            var lastMonthTotalInvestment = lastMonthConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.GlobalTotalInvestment.ToString())?.Amount.Amount ?? 0;
            var lastMonthTotalSavings = lastMonthConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.GlobalTotalSavings.ToString())?.Amount.Amount ?? 0;
            var lastMonthActiveDebt = lastMonthConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.GlobalActiveDebt.ToString())?.Amount.Amount ?? 0;

            var globalNetCashFlow = currentNetCashFlow + lastMonthNetCashFlow;
            var globalTotalInvestments = CurrentAggregates.TotalInvestments + lastMonthTotalInvestment;
            var globalTotalSaving = CurrentAggregates.TotalSavings + lastMonthTotalSavings;
            var globalActiveDebt = currentActiveDebt + lastMonthActiveDebt;

            return
            [
                new() { Period = period, PeriodType= AggregateType.GlobalAggregate, Name = AggregateName.GlobalNetCashFlow.ToString(), Amount = new Money(globalNetCashFlow,currency)},
                new() { Period = period,PeriodType= AggregateType.GlobalAggregate, Name = AggregateName.GlobalTotalInvestment.ToString(), Amount = new Money(globalTotalInvestments,currency)},
                new() { Period = period, PeriodType= AggregateType.GlobalAggregate, Name = AggregateName.GlobalTotalSavings.ToString(), Amount = new Money(globalTotalSaving,currency)},
                new() { Period = period, PeriodType= AggregateType.GlobalAggregate, Name = AggregateName.GlobalActiveDebt.ToString(), Amount = new Money(globalActiveDebt,currency)},
            ];
        }

        private async Task<List<FinancialAggregateDto>> CalculateMonthlyAggregatesAsync(Guid userId, DateTime month)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if(month == DateTime.MinValue)
            {
                throw new ArgumentException("A valid month must be provided.", nameof(month));
            }

            var lastMonth = month.AddMonths(-1);
            var transactions = await financialTransactionRepository.GetByMonthAsync(userId, month) ?? throw new NotFoundException("Failed to retrieve transactions for the specified month.");
            var convertedTransactions = await currencyConversions.ConvertTransactionBaseCurrencyAsync(userId, transactions) ?? throw new NotFoundException("Failed to convert transactions to base currency.");

            var lastMonthAggregates = await financialAggregatesRepository.GetFinancialAggregatesByMonthAsync(userId, lastMonth) ?? throw new NotFoundException("Failed to retrieve monthly Aggregates for the specified month.");

            var startDate = new DateOnly(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var period = new Period(startDate, endDate);

            return CalculateFinancialAggregates(userId, convertedTransactions, lastMonthAggregates, period);
        }

        private List<FinancialAggregateDto> CalculateFinancialAggregates(Guid userId, List<FinancialTransaction> transactions,List<FinancialAggregate> lastMonthAggregates, Period period)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            
            var currency = new Currency(transactions.FirstOrDefault()?.BaseAmount?.Currency?.Code ?? "USD", transactions.FirstOrDefault()?.BaseAmount?.Currency?.Symbol ?? "$");

            var AggregatesByType = CalculateAggregatesByType(transactions);

            var totalEssentialExpenses = transactions.Where(t => t.Type == FinancialType.Expense && t.IsEssential).Sum(t => t.BaseAmount.Amount);
            var totalDiscretionaryExpenses = transactions.Where(t => t.Type == FinancialType.Expense && !t.IsEssential).Sum(t => t.BaseAmount.Amount);

            var calculatedAggregates = new List<FinancialAggregateDto>
                {
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalIncome.ToString(), Amount = new Money(AggregatesByType?.TotalIncome ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalIncome.ToString())?.Amount.Amount ?? 0,AggregatesByType?.TotalIncome ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalExpense.ToString(), Amount = new Money(AggregatesByType?.TotalExpenses ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalExpense.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalExpenses ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalInvestment.ToString(), Amount = new Money(AggregatesByType ?.TotalInvestments ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalInvestment.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalInvestments ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalSavings.ToString(), Amount = new Money(AggregatesByType?.TotalSavings ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(Ag => Ag.Name == AggregateName.TotalSavings.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalSavings ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalContractedLoans.ToString(), Amount = new Money(AggregatesByType?.TotalContractedLoans ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalContractedLoans.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalContractedLoans ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalInterestPayments.ToString(), Amount = new Money(AggregatesByType?.TotalInterestPayments ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalInterestPayments.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalInterestPayments ?? 0) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalPrincipalRepayments.ToString(), Amount = new Money(AggregatesByType?.TotalPrincipalRepayments ?? 0, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalPrincipalRepayments.ToString())?.Amount.Amount ?? 0, AggregatesByType?.TotalPrincipalRepayments ?? 0) },
                    new() {  Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalEssentialExpenses.ToString(), Amount = new Money(totalEssentialExpenses, currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalEssentialExpenses.ToString())?.Amount.Amount ?? 0, totalEssentialExpenses) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.NetBalance.ToString(), Amount = new Money(financialCalculations.GetNetBalance(AggregatesByType?.TotalIncome ?? 0, AggregatesByType?.TotalExpenses ?? 0), currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.NetBalance.ToString())?.Amount.Amount ?? 0, financialCalculations.GetNetBalance(AggregatesByType?.TotalIncome?? 0, AggregatesByType?.TotalExpenses ?? 0)) },
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.NetCashFlow.ToString(), Amount = new Money( financialCalculations.GetNetCashFlow(AggregatesByType?.TotalIncome ?? 0, AggregatesByType?.TotalExpenses ?? 0, AggregatesByType?.TotalInvestments ?? 0, AggregatesByType?.TotalSavings ?? 0, AggregatesByType?.TotalContractedLoans?? 0, AggregatesByType?.TotalInterestPayments ?? 0, AggregatesByType?.TotalPrincipalRepayments ?? 0), currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.NetCashFlow.ToString())?.Amount.Amount ?? 0,financialCalculations.GetNetCashFlow(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalExpenses ?? 0, AggregatesByType ?.TotalInvestments ?? 0, AggregatesByType ?.TotalSavings ?? 0, AggregatesByType ?.TotalContractedLoans ?? 0, AggregatesByType ?.TotalInterestPayments ?? 0, AggregatesByType ?.TotalPrincipalRepayments ?? 0) ) },
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalInflow.ToString(), Amount = new Money(financialCalculations.GetTotalInflow(AggregatesByType?.TotalIncome ?? 0, AggregatesByType ?.TotalContractedLoans ?? 0), currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalInflow.ToString())?.Amount.Amount ?? 0, financialCalculations.GetTotalInflow(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalContractedLoans ?? 0))},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.TotalOutflow.ToString(), Amount = new Money(financialCalculations.GetTotalOutflow(AggregatesByType ?.TotalExpenses ?? 0, AggregatesByType ?.TotalInvestments ?? 0, AggregatesByType ?.TotalSavings ?? 0, AggregatesByType ?.TotalInterestPayments ?? 0, AggregatesByType ?.TotalPrincipalRepayments ?? 0), currency), Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name == AggregateName.TotalOutflow.ToString())?.Amount.Amount ?? 0, financialCalculations.GetTotalOutflow(AggregatesByType ?.TotalExpenses ?? 0, AggregatesByType ?.TotalInvestments ?? 0, AggregatesByType ?.TotalSavings ?? 0, AggregatesByType ?.TotalInterestPayments ?? 0, AggregatesByType ?.TotalPrincipalRepayments ?? 0))},
                    new() { Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.SavingsRate.ToString(), Amount = new Money(financialCalculations.GetSavingsRate(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalSavings ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.EssentialExpensesRatio.ToString(), Amount = new Money(financialCalculations.GetEssentialExpensesRatio(AggregatesByType ?.TotalIncome ?? 0, totalEssentialExpenses), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.DiscretionaryExpensesRatio.ToString(), Amount = new Money(financialCalculations.GetDiscretionaryExpensesRatio(AggregatesByType ?.TotalIncome ?? 0, totalDiscretionaryExpenses), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.DicretionaryToExpenseRatio.ToString(), Amount = new Money(financialCalculations.GetDiscretionaryToExpensesRatio(totalDiscretionaryExpenses, AggregatesByType ?.TotalExpenses ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.ExpensesRatio.ToString(), Amount = new Money(financialCalculations.GetExpensesRatio(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalExpenses ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.InvestmentRatio.ToString(), Amount = new Money(financialCalculations.GetInvestmentRatio(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalInvestments ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.DebtRatio.ToString(), Amount = new Money(financialCalculations.GetDebtRatio(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalContractedLoans ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.DebtPaymentRatio.ToString(), Amount = new Money(financialCalculations.GetDebtPaymentRatio(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalPrincipalRepayments ?? 0, AggregatesByType ?.TotalInterestPayments ?? 0), null)},
                    new() {Period = period, PeriodType= AggregateType.MonthlyAggregate, Name = AggregateName.InterestBurdenRatio.ToString(), Amount = new Money(financialCalculations.GetInterestBurdenRatio(AggregatesByType ?.TotalIncome ?? 0, AggregatesByType ?.TotalInterestPayments ?? 0), null)},
                };
            var transactionsByCategory = transactions.GroupBy(t => t.Category.Name).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var group in transactionsByCategory)
            {
                var groupAggregate = new FinancialAggregateDto
                {
                    Period = period,
                    PeriodType = AggregateType.MonthlyAggregate,
                    Type = group.Value?[0].Type,
                    Name = group.Key,
                    Amount = new Money(group.Value?.Sum(t => t.BaseAmount.Amount) ?? 0, currency),
                    Variation = financialCalculations.GetIndicatorGrowth(lastMonthAggregates?.FirstOrDefault(ag => ag.Name.ToString() == group.Key)?.Amount.Amount ?? 0, group.Value?.Sum(t => t.BaseAmount.Amount) ?? 0)
                };
                calculatedAggregates.Add(groupAggregate);
            }

            return calculatedAggregates;
        }


        private async Task<List<AggregateVariationsDto>> GetAggregateVariationsByMonthAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            var monthlyIncomeVariations = await currencyConversions.ConvertAggregateBaseCurrencyAsync(userId, await financialAggregatesRepository.GetFinancialAggregatesByNameAsync(userId, AggregateName.TotalIncome.ToString()));
            var monthlyExpenseVariations = await currencyConversions.ConvertAggregateBaseCurrencyAsync(userId, await financialAggregatesRepository.GetFinancialAggregatesByNameAsync(userId, AggregateName.TotalExpense.ToString()));
            var monthlySalaryVariations = await currencyConversions.ConvertAggregateBaseCurrencyAsync(userId, await financialAggregatesRepository.GetFinancialAggregatesByNameAsync(userId, "Salary"));
            var monthlyInvestmentVariations = await currencyConversions.ConvertAggregateBaseCurrencyAsync(userId, await financialAggregatesRepository.GetFinancialAggregatesByNameAsync(userId, AggregateName.TotalInvestment.ToString()));
            return
            [
                new() {Name = "Income Variations", Values = [..monthlyIncomeVariations.Select(ag => new AggregateVariationsValueByMonthDto
                {
                    Period = ag.Period,
                    PeriodType = ag.PeriodType,
                    Type = ag.Type,
                    Amount = ag.Amount
                })]},
                new() {Name = "Expense Variations", Values = [..monthlyExpenseVariations.Select(ag => new AggregateVariationsValueByMonthDto
                {
                    Period = ag.Period,
                    PeriodType = ag.PeriodType,
                    Type = ag.Type,
                    Amount = ag.Amount
                })]},
                new() {Name = "Salary Variations", Values = [..monthlySalaryVariations.Select(ag => new AggregateVariationsValueByMonthDto
                {
                    Period = ag.Period,
                    PeriodType = ag.PeriodType,
                    Type = ag.Type,
                    Amount = ag.Amount
                })]},
                new() {Name = "Investment Variations", Values = [..monthlyInvestmentVariations.Select(ag => new AggregateVariationsValueByMonthDto
                {
                    Period = ag.Period,
                    PeriodType = ag.PeriodType,
                    Type = ag.Type,
                    Amount = ag.Amount
                })]},
            ]; 
        }

        private static AggregatesByTypeDto CalculateAggregatesByType(List<FinancialTransaction> transactions)
        {

            return new AggregatesByTypeDto
            {
                TotalIncome = transactions?.Where(tr => tr.Type == FinancialType.Income).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalExpenses = transactions?.Where(tr => tr.Type == FinancialType.Expense).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalInvestments = transactions?.Where(tr => tr.Type == FinancialType.Investment).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalSavings = transactions?.Where(tr => tr.Type == FinancialType.Savings).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalContractedLoans = transactions?.Where(tr => tr.Type == FinancialType.ContractedLoan).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalInterestPayments = transactions?.Where(tr => tr.Type == FinancialType.InterestPayment).Sum(tr => tr.BaseAmount.Amount) ?? 0,
                TotalPrincipalRepayments = transactions?.Where(tr => tr.Type == FinancialType.PrincipalRepayment).Sum(tr => tr.BaseAmount.Amount) ?? 0
            };
        }

         
    }
}
//TODO: create a factory design to handle yearly/monthly aggregates and an abstract class to share private methods.