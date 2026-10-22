using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Services
{
    public class FinancialAggregatesService(IFinancialAggregatesRepository financialAggregatesRepository,
        IFinancialCalculations financialCalculations,
        IFinancialTransactionRepository financialTransactionRepository,
        ICurrencyConversions currencyConversions
        ) : IFinancialAggregatesService
    {
        public async Task<GlobalAggregatesResponseDto> GetGlobalAggregates(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            var currentYear = DateTime.UtcNow;
            var lastYear = DateTime.UtcNow.Year - 1;
            var lastYearAggregates = await financialAggregatesRepository.GetFinancialAggregatesByYearAsync(userId, lastYear) ?? throw new NotFoundException("Failed to retrieve last year's aggregates") ;

            var transactions = await financialTransactionRepository.GetByYear(userId, currentYear) ?? throw new NotFoundException("Failed to retrieve current year's transactions");

            var convertedTransactions = await currencyConversions.ConvertTransactionBaseCurrency(userId, transactions) ?? throw new NotFoundException("Failed to convert current year's transactions to base currency");

            

            var lastYearConvertedAggregates = await currencyConversions.ConvertAggregateBaseCurrency(userId, lastYearAggregates) ?? throw new NotFoundException("Failed to convert last year's aggregates to base currency");

            var currentTotalIncome = convertedTransactions.Where(t => t.Type == FinancialType.Income).Sum(t => t.BaseAmount);
            var currentTotalExpense = convertedTransactions.Where(t => t.Type == FinancialType.Expense).Sum(t => t.BaseAmount);
            var currentTotalInvestment = convertedTransactions.Where(t => t.Type == FinancialType.Investment).Sum(t => t.BaseAmount);
            var currentTotalSavings = convertedTransactions.Where(t => t.Type == FinancialType.Savings).Sum(t => t.BaseAmount);
            var currentTotalContractedLoans = convertedTransactions.Where(t => t.Type == FinancialType.ContractedLoan).Sum(t => t.BaseAmount);
            var currentTotalInterestPayments = convertedTransactions.Where(t => t.Type == FinancialType.InterestPayment).Sum(t => t.BaseAmount);
            var currentTotalPrincipalRepayments = convertedTransactions.Where(t => t.Type == FinancialType.PrincipalRepayment).Sum(t => t.BaseAmount);



            var currentTotalActiveDebt = currentTotalContractedLoans - currentTotalPrincipalRepayments;
            var currentYearNetCashFlow = financialCalculations.GetNetCashFlow(currentTotalIncome, currentTotalExpense, currentTotalInvestment, currentTotalSavings, currentTotalContractedLoans, currentTotalInterestPayments, currentTotalPrincipalRepayments);
            
            var lastYearNetCashFlow = lastYearConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.NetCashFlow)?.Value ?? 0;
            var lastYearTotalInvestment = lastYearConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalInvestment)?.Value ?? 0;
            var lastYearTotalSavings = lastYearConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalSavings)?.Value ?? 0;
            var lastYearActiveDebt = lastYearConvertedAggregates.FirstOrDefault(a => a.Name == AggregateName.ActiveDebt)?.Value ?? 0;

            var totalCashFlow = currentYearNetCashFlow + lastYearNetCashFlow;
            var totalInvestments = currentTotalInvestment + lastYearTotalInvestment;
            var totalSaving = currentTotalSavings + lastYearTotalSavings;
            var totalActiveDebts = currentTotalActiveDebt + lastYearActiveDebt;

            return new GlobalAggregatesResponseDto
            {
                Date = DateTime.UtcNow,
                TotalCashFlow = totalCashFlow,
                TotalInvestment = totalInvestments,
                TotalSavings = totalSaving,
                TotalActiveDebt = totalActiveDebts
            };
        }

        public async Task<FinancialAggregatesResponseDto> GetAggregatesByFilter(Guid userId, FinancialAggregatesFilterDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            if (dto.AggregatesPeriod == TransactionPeriod.Monthly)
            {
                return await GetAggregatesPerMonth(userId, dto.Date);
            }  
            else
            {
                throw new ArgumentException("Invalid period. Must be 'month' or 'year'.");
            }
        }

        private async Task<FinancialAggregatesResponseDto> GetAggregatesPerMonth(Guid userId, DateTime date)
        {
            if(date == DateTime.MinValue)
            {
                throw new ArgumentException("Date cannot be the default value.");
            }
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            var previousMonth = date.AddMonths(-1);

            var currentMonthTransactions = await financialTransactionRepository.GetByMonth(userId, date) ?? throw new NotFoundException("Failed to retrieve current month's transactions");
            var previousMonthTransactions = await financialTransactionRepository.GetByMonth(userId, previousMonth) ?? throw new NotFoundException("Failed to retrieve previous month's transactions");

            var currentMonthConvertedTransactions = await currencyConversions.ConvertTransactionBaseCurrency(userId, currentMonthTransactions) ?? throw new NotFoundException("Failed to convert current month's transactions to base currency");
            var previousMonthConvertedTransactions = await currencyConversions.ConvertTransactionBaseCurrency(userId, previousMonthTransactions) ?? throw new NotFoundException("Failed to convert previous month's transactions to base currency");

            var currentTotals = CalculateTransactionTotals(currentMonthConvertedTransactions) ?? throw new NotFoundException("Failed to calculate current month's transaction totals");
            var previousTotals = CalculateTransactionTotals(previousMonthConvertedTransactions) ?? throw new NotFoundException("Failed to calculate previous month's transaction totals");

            var currentAggregates = CalculateAggregates(currentTotals);
            var previousAggregates = CalculateAggregates(previousTotals);

            var variations = CalculateVarations(currentTotals, currentAggregates, previousTotals, previousAggregates);

            var aggregatesVariationByMonth = await GetAggregatesByMonthPeriod(userId) ?? throw new NotFoundException("Failed to retrieve aggregates by month period");

            return
                new FinancialAggregatesResponseDto
                {
                    Date = date,
                    Totals = currentTotals,
                    Aggregates = currentAggregates,
                    Variations = variations,
                    AggregatesVariationsByMonth = aggregatesVariationByMonth
                };   
        }


        private static List<FinancialTotalDto> CalculateTransactionTotals( List<FinancialTransaction> transactions)
        {
            ArgumentNullException.ThrowIfNull(transactions);
            

            var transactionsByType = transactions.GroupBy(t => t.Type).ToDictionary(g => g.Key, g => g.ToList());
            var transactionsByCategory = transactions.GroupBy(t => t.Category.Name).ToDictionary(g => g.Key, g => g.ToList());

            var totalImportantExpenses = transactions.Where(t => t.IsEssential && t.Type == FinancialType.Expense).Sum(t => t.BaseAmount);

            var totals = new List<FinancialTotalDto>();

            foreach (var item in transactionsByType)
            {
                var dtoItem = new FinancialTotalDto
                {
                    Name = item.Key.ToString(),
                    Filter = TotalType.ByType,
                    Amount = item.Value.Sum(t => t.BaseAmount)
                };
                totals.Add(dtoItem);
            }

            foreach (var item in transactionsByCategory)
            {
                var dtoItem = new FinancialTotalDto
                {
                    Type = item.Value[0].Type,
                    Name = item.Key,
                    Filter = TotalType.ByCategory,
                    Amount = item.Value.Sum(t => t.BaseAmount)
                };
                totals.Add(dtoItem);
            }

            totals.Add(new FinancialTotalDto
            {
                Type = FinancialType.Expense,
                Name = "Important Expenses",
                Filter = TotalType.ByImportance,
                Amount = totalImportantExpenses
            });

            return totals;
        }

        private CalculatedAggregatesDto CalculateAggregates(List<FinancialTotalDto> totals)
        {
            ArgumentNullException.ThrowIfNull(totals);

            var totalIncome = GetTotal(FinancialType.Income, totals);
            var totalExpenses = GetTotal(FinancialType.Expense, totals);
            var totalInvestments = GetTotal(FinancialType.Investment, totals);
            var totalSavings = GetTotal(FinancialType.Savings, totals);
            var totalLoans = GetTotal(FinancialType.ContractedLoan, totals);
            var totalInterestPayments = GetTotal(FinancialType.InterestPayment, totals);
            var totalPrincipalRepayments = GetTotal(FinancialType.PrincipalRepayment, totals);
            var totalImportantExpenses = totals.Where(t => t.Name == "Important Expenses").Select(t => t.Amount).FirstOrDefault();

            var NetBalance = financialCalculations.GetNetBalance(totalIncome, totalExpenses );
            var NetCashFlow = financialCalculations.GetNetCashFlow(totalIncome, totalExpenses, totalInvestments, totalSavings, totalLoans, totalInterestPayments, totalPrincipalRepayments);
            var TotalInflow = financialCalculations.GetTotalInflow(totalIncome, totalLoans);
            var TotalOutflow = financialCalculations.GetTotalOutflow(totalExpenses, totalInvestments, totalSavings, totalInterestPayments, totalPrincipalRepayments);
            var SavingsRate = financialCalculations.GetSavingsRate(totalIncome, totalSavings);
            var EssentialExpensesRatio = financialCalculations.GetEssentialExpensesRatio(totalIncome, totalImportantExpenses);
            var ExpensesRatio = financialCalculations.GetExpensesRatio(totalIncome, totalExpenses);
            var InvestmentRatio = financialCalculations.GetInvestmentRatio(totalIncome, totalInvestments);
            var DebtRatio = financialCalculations.GetDebtRatio(totalIncome, totalLoans);
            var DebtPaymentRatio = financialCalculations.GetDebtPaymentRatio(totalIncome, totalPrincipalRepayments, totalInterestPayments);
            var InterestBurdenRatio = financialCalculations.GetInterestBurdenRatio(totalIncome, totalInterestPayments);

            return new CalculatedAggregatesDto
            {
                NetBalance = NetBalance,
                NetCashFlow = NetCashFlow,
                TotalInflow = TotalInflow,
                TotalOutflow = TotalOutflow,
                SavingsRate = SavingsRate,
                EssentialExpensesRatio = EssentialExpensesRatio,
                ExpensesRatio = ExpensesRatio,
                InvestmentRatio = InvestmentRatio,
                DebtRatio = DebtRatio,
                DebtPaymentRatio = DebtPaymentRatio,
                InterestBurdenRatio = InterestBurdenRatio
            };

        }

        private AggregateVariationDto CalculateVarations (List<FinancialTotalDto> currentTotals, CalculatedAggregatesDto currentAggregates, List<FinancialTotalDto> previousTotals, CalculatedAggregatesDto previousAggregates)
        {
            ArgumentNullException.ThrowIfNull(currentTotals);
            ArgumentNullException.ThrowIfNull(previousTotals);
            ArgumentNullException.ThrowIfNull(currentAggregates);
            ArgumentNullException.ThrowIfNull(previousAggregates);

            var currentTotalIncome = GetTotal(FinancialType.Income, currentTotals);
            var previousTotalIncome = GetTotal(FinancialType.Income, previousTotals);
            var currentTotalExpenses = GetTotal(FinancialType.Expense, currentTotals);
            var previousTotalExpenses = GetTotal(FinancialType.Expense, previousTotals);
            var currentTotalSavings = GetTotal(FinancialType.Savings, currentTotals);
            var previousTotalSavings = GetTotal(FinancialType.Savings, previousTotals);
            var currentTotalInvestments = GetTotal(FinancialType.Investment, currentTotals);
            var previousTotalInvestments = GetTotal(FinancialType.Investment, previousTotals);
            var currentContractedLoan = GetTotal(FinancialType.ContractedLoan, currentTotals);
            var previousContractedLoan = GetTotal(FinancialType.ContractedLoan, previousTotals);

            return new AggregateVariationDto
            {
                IncomeVariation = financialCalculations.GetIndicatorGrowth(previousTotalIncome, currentTotalIncome),
                ExpensesVariation = financialCalculations.GetIndicatorGrowth(previousTotalExpenses, currentTotalExpenses),
                SavingsVariation = financialCalculations.GetIndicatorGrowth(previousTotalSavings, currentTotalSavings),
                InvestmentVariation = financialCalculations.GetIndicatorGrowth(previousTotalInvestments, currentTotalInvestments),
                DebtVariation = financialCalculations.GetIndicatorGrowth(previousContractedLoan, currentContractedLoan),
                NetBalanceVariation = financialCalculations.GetIndicatorGrowth(previousAggregates.NetBalance, currentAggregates.NetBalance),
                NetCashFlowVariation = financialCalculations.GetIndicatorGrowth(previousAggregates.NetCashFlow, currentAggregates.NetCashFlow),
            };
        }

        private async Task<List<AggregateByPeriodDto>> GetAggregatesByMonthPeriod(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.");
            }
            var incomeMonthlyVariations = await financialTransactionRepository.GetByTypeByMonth(userId, FinancialType.Income);
            var expenseMonthlyVariations = await financialTransactionRepository.GetByTypeByMonth(userId, FinancialType.Expense);
            var savingsMonthlyVariations = await financialTransactionRepository.GetByTypeByMonth(userId, FinancialType.Savings);
            var investmentMonthlyVariations = await financialTransactionRepository.GetByTypeByMonth(userId, FinancialType.Investment);
            var contractedLoanMonthlyVariations = await financialTransactionRepository.GetByTypeByMonth(userId, FinancialType.ContractedLoan);

            var salaryMonthlyVariations = await financialTransactionRepository.GetByCategoryByMonth(userId, "Salary");

            return 
                incomeMonthlyVariations
                .Concat(expenseMonthlyVariations)
                .Concat(savingsMonthlyVariations)
                .Concat(investmentMonthlyVariations)
                .Concat(contractedLoanMonthlyVariations)
                .Concat(salaryMonthlyVariations)
                .ToList();

        }

        private static decimal GetTotal(FinancialType type, List<FinancialTotalDto> totals)
        {
            return totals.Where(t => t.Name == type.ToString()).Select(t => t.Amount).FirstOrDefault();
        }
         
    }
}
//TODO: create a factory design to handle yearly/monthly aggregates and an abstract class to share private methods.