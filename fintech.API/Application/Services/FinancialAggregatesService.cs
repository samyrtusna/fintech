using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
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
            var currentYear = DateTime.UtcNow;
            var lastYear = DateTime.UtcNow.Year - 1;
            var lastYearAggregates = await financialAggregatesRepository.GetFinancialAggregatesByYearAsync(userId, lastYear) ?? throw new NotFoundException("Failed to retrieve last year's aggregates") ;

            var transactions = await GetTransactionsByTypeByYear(userId, currentYear) ?? throw new NotFoundException("Failed to retrieve current year's transactions");

            var totals = await CalculateTransactionTotals(userId, transactions) ?? throw new NotFoundException("Failed to calculate transaction totals");


            var totalActiveDebt = totals.TotalContractedLoans - totals.TotalPrincipalRepayments;
            var currentYearNetCashFlow = financialCalculations.GetNetCashFlow(totals.TotalIncome, totals.TotalExpense, totals.TotalInvestment, totals.TotalSavings, totals.TotalContractedLoans, totals.TotalInterestPayments, totals.TotalPrincipalRepayments);
            
            var lastYearNetCashFlow = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.NetCashFlow)?.Value ?? 0;
            var lastYearTotalInvestment = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalInvestment)?.Value ?? 0;
            var lastYearTotalSavings = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.TotalSavings)?.Value ?? 0;
            var lastYearActiveDebt = lastYearAggregates.FirstOrDefault(a => a.Name == AggregateName.ActiveDebt)?.Value ?? 0;

            var totalCashFlow = currentYearNetCashFlow + lastYearNetCashFlow;
            var totalInvestments = totals.TotalInvestment + lastYearTotalInvestment;
            var totalSavings = totals.TotalSavings + lastYearTotalSavings;
            var totalActiveDebts = totalActiveDebt + lastYearActiveDebt;

            return new GlobalAggregatesResponseDto
            {
                Date = DateTime.UtcNow,
                TotalCashFlow = totalCashFlow,
                TotalInvestment = totalInvestments,
                TotalSavings = totalSavings,
                TotalActiveDebt = totalActiveDebts
            };
        }

        public async Task<FinancialAggregatesResponseDto[]> GetAggregatesByFilter(Guid userId, FinancialAggregatesFilterDto dto)
        {
            if (dto.AggregatesPeriod == TransactionPeriod.Monthly)
            {
                return await GetAggregatesPerMonth(userId, dto.Date);
            }  
            else if (dto.AggregatesPeriod == TransactionPeriod.Yearly)
            {
                return await GetAggregatesPerYear(userId, dto.Date);
            }
            else
            {
                throw new ArgumentException("Invalid period. Must be 'month' or 'year'.");
            }
        }

        private async Task<FinancialAggregatesResponseDto[]> GetAggregatesPerMonth(Guid userId, DateTime date)
        {
            var previousMonth = date.AddMonths(-1);

            var currenttransactions = await GetTransactionsByTypeByMonth(userId, date) ?? throw new NotFoundException("Failed to retrieve current month's transactions");
            var previousTransactions = await GetTransactionsByTypeByMonth(userId, previousMonth) ?? throw new NotFoundException("Failed to retrieve previous month's transactions");

            var currentTotals = await CalculateTransactionTotals(userId, currenttransactions) ?? throw new NotFoundException("Failed to calculate current month's transaction totals");
            var previousTotals = await CalculateTransactionTotals(userId, previousTransactions) ?? throw new NotFoundException("Failed to calculate previous month's transaction totals");

            var currentAggregates = CalculateAggregates(currentTotals);
            var previousAggregates = CalculateAggregates(previousTotals);

            var variations = CalculateVarations(currentTotals, currentAggregates, previousTotals, previousAggregates);

            return  ReturnAggregates(currentTotals, currentAggregates, variations, date);
        }

        private async Task<FinancialAggregatesResponseDto[]> GetAggregatesPerYear(Guid userId, DateTime date)
        {
            var previousYear = date.AddYears(-1);
            var currenttransactions = await GetTransactionsByTypeByYear(userId, date);
            var previousTransactions = await GetTransactionsByTypeByYear(userId, previousYear);

            var currentTotals = await CalculateTransactionTotals(userId, currenttransactions);
            var previousTotals = await CalculateTransactionTotals(userId, previousTransactions);

            var currentAggregates = CalculateAggregates(currentTotals);
            var previousAggregates = CalculateAggregates(previousTotals);

            var variations = CalculateVarations(currentTotals, currentAggregates, previousTotals, previousAggregates);

            return  ReturnAggregates(currentTotals, currentAggregates, variations, date);
        }

        private async Task<TransactionsByTypesDto> GetTransactionsByTypeByYear(Guid userId, DateTime date)
        {
            var incomeTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.Income);
            var expenseTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.Expense);
            var investmentTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.Investment);
            var savingTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.Savings);
            var contractedLoanTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.ContractedLoan);
            var interestPaymentTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.InterestPayment);
            var principalRepaymentTransactions = await financialTransactionRepository.GetByTypeByYear(userId, date, FinancialType.PrincipalRepayment);

            return new TransactionsByTypesDto
            {
                IncomeTransactions = incomeTransactions,
                ExpenseTransactions = expenseTransactions,
                InvestmentTransactions = investmentTransactions,
                SavingTransactions = savingTransactions,
                ContractedLoanTransactions = contractedLoanTransactions,
                InterestPaymentTransactions = interestPaymentTransactions,
                PrincipalRepaymentTransactions = principalRepaymentTransactions
            };
        }

        private async Task<TransactionsByTypesDto> GetTransactionsByTypeByMonth(Guid userId, DateTime date)
        {
            var incomeTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.Income);
            var expenseTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.Expense);
            var investmentTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.Investment);
            var savingTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.Savings);
            var contractedLoanTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.ContractedLoan);
            var interestPaymentTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.InterestPayment);
            var principalRepaymentTransactions = await financialTransactionRepository.GetByTypeByMonth(userId, date, FinancialType.PrincipalRepayment);


            return new TransactionsByTypesDto
            {
                IncomeTransactions = incomeTransactions,
                ExpenseTransactions = expenseTransactions,
                InvestmentTransactions = investmentTransactions,
                SavingTransactions = savingTransactions,
                ContractedLoanTransactions = contractedLoanTransactions,
                InterestPaymentTransactions = interestPaymentTransactions,
                PrincipalRepaymentTransactions = principalRepaymentTransactions
            };
        }

        private async Task<FinancialTotalsDto> CalculateTransactionTotals(Guid userId, TransactionsByTypesDto dto)
        {
            var incomeTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.IncomeTransactions);
            var expenseTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.ExpenseTransactions);
            var investmentTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.InvestmentTransactions);
            var savingTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.SavingTransactions);
            var contractedLoanTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.ContractedLoanTransactions);
            var interestPaymentTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.InterestPaymentTransactions);
            var principalRepaymentTransactions = await currencyConversions.ConvertToBaseCurrency(userId, dto.PrincipalRepaymentTransactions);

            return new FinancialTotalsDto
            {
                TotalIncome = incomeTransactions.Sum(t => t.BaseAmount),
                TotalExpense = expenseTransactions.Sum(t => t.BaseAmount),
                TotalInvestment = investmentTransactions.Sum(t => t.BaseAmount),
                TotalSavings = savingTransactions.Sum(t => t.BaseAmount),
                TotalContractedLoans = contractedLoanTransactions.Sum(t => t.BaseAmount),
                TotalInterestPayments = interestPaymentTransactions.Sum(t => t.BaseAmount),
                TotalPrincipalRepayments = principalRepaymentTransactions.Sum(t => t.BaseAmount)
            };
        }

        private CalculatedAggregatesDto CalculateAggregates(FinancialTotalsDto totals)
        {
            return new CalculatedAggregatesDto {
                NetBalance = financialCalculations.GetNetBalance(totals.TotalIncome, totals.TotalExpense),
                NetCashFlow = financialCalculations.GetNetCashFlow(totals.TotalIncome, totals.TotalExpense, totals.TotalInvestment, totals.TotalSavings, totals.TotalContractedLoans, totals.TotalInterestPayments, totals.TotalPrincipalRepayments),
                TotalInflow = financialCalculations.GetTotalInflow(totals.TotalIncome, totals.TotalContractedLoans),
                TotalOutflow = financialCalculations.GetTotalOutflow(totals.TotalExpense, totals.TotalInvestment, totals.TotalSavings, totals.TotalInterestPayments, totals.TotalPrincipalRepayments),
                SavingsRate = financialCalculations.GetSavingsRate(totals.TotalIncome, totals.TotalSavings),
                EssentialExpensesRatio = financialCalculations.GetEssentialExpensesRatio(totals.TotalIncome, totals.TotalExpense),
                ExpensesRatio = financialCalculations.GetExpensesRatio(totals.TotalIncome, totals.TotalExpense),
                InvestmentRatio = financialCalculations.GetInvestmentRatio(totals.TotalIncome, totals.TotalInvestment),
                DebtRatio = financialCalculations.GetDebtRatio(totals.TotalIncome, totals.TotalContractedLoans),
                DebtPaymentRatio = financialCalculations.GetDebtPaymentRatio(totals.TotalIncome, totals.TotalPrincipalRepayments, totals.TotalInterestPayments),
                InterestBurdenRatio = financialCalculations.GetInterestBurdenRatio(totals.TotalIncome, totals.TotalInterestPayments)
            };                  
        }

        private AggregateVariationDto CalculateVarations (FinancialTotalsDto currentTotals, CalculatedAggregatesDto currentAggregates, FinancialTotalsDto previousTotals, CalculatedAggregatesDto previousAggregates)
        {
            return new AggregateVariationDto
            {
                IncomeVariation = financialCalculations.GetIndicatorGrowth(previousTotals.TotalIncome, currentTotals.TotalIncome),
                ExpensesVariation = financialCalculations.GetIndicatorGrowth(previousTotals.TotalExpense, currentTotals.TotalExpense),
                SavingsVariation = financialCalculations.GetIndicatorGrowth(previousTotals.TotalSavings, currentTotals.TotalSavings),
                InvestmentVariation = financialCalculations.GetIndicatorGrowth(previousTotals.TotalInvestment, currentTotals.TotalInvestment),
                DebtVariation = financialCalculations.GetIndicatorGrowth(previousTotals.TotalContractedLoans, currentTotals.TotalContractedLoans),
                NetBalanceVariation = financialCalculations.GetIndicatorGrowth(previousAggregates.NetBalance, currentAggregates.NetBalance),
                NetCashFlowVariation = financialCalculations.GetIndicatorGrowth(previousAggregates.NetCashFlow, currentAggregates.NetCashFlow),
            };
        }

        private FinancialAggregatesResponseDto[] ReturnAggregates (FinancialTotalsDto totals, CalculatedAggregatesDto calculatedAggregates, AggregateVariationDto variations, DateTime date)
        {
            return [

            new() { Date = date, Name = AggregateName.TotalIncome, Value = totals.TotalIncome, Variation= variations.IncomeVariation},
                new() { Date = date, Name = AggregateName.TotalExpense, Value = totals.TotalExpense, Variation= variations.ExpensesVariation},
                new() { Date = date, Name = AggregateName.TotalInvestment, Value = totals.TotalInvestment, Variation= variations.InvestmentVariation},
                new() { Date = date, Name = AggregateName.TotalInflow, Value = calculatedAggregates.TotalInflow},
                new() { Date = date, Name = AggregateName.TotalOutflow, Value = calculatedAggregates.TotalOutflow},
                new() { Date = date, Name = AggregateName.TotalSavings, Value = totals.TotalSavings, Variation= variations.SavingsVariation },
                new() { Date = date, Name = AggregateName.TotalContractedLoans, Value = totals.TotalContractedLoans, Variation= variations.DebtVariation },
                new() { Date = date, Name = AggregateName.TotalInterestPayments, Value = totals.TotalInterestPayments},
                new() { Date = date, Name = AggregateName.TotalPrincipalRepayments, Value = totals.TotalPrincipalRepayments},
                new() { Date = date, Name = AggregateName.NetBalance, Value = calculatedAggregates.NetBalance, Variation = variations.NetBalanceVariation},
                new() { Date = date, Name = AggregateName.NetCashFlow, Value = calculatedAggregates.NetCashFlow, Variation= variations.NetCashFlowVariation},
                new() { Date = date, Name = AggregateName.SavingsRate, Value = calculatedAggregates.SavingsRate},
                new() { Date = date, Name = AggregateName.EssentialExpensesRatio, Value = calculatedAggregates.EssentialExpensesRatio},
                new() { Date = date, Name = AggregateName.ExpensesRatio, Value = calculatedAggregates.ExpensesRatio},
                new() { Date = date, Name = AggregateName.InvestmentRatio, Value = calculatedAggregates.InvestmentRatio},
                new() { Date = date, Name = AggregateName.DebtRatio, Value = calculatedAggregates.DebtRatio},
                new() { Date = date, Name = AggregateName.DebtPaymentRatio, Value = calculatedAggregates.DebtPaymentRatio},
                new() { Date = date, Name = AggregateName.InterestBurdenRatio, Value = calculatedAggregates.InterestBurdenRatio }
        ];
        }
    }
}
