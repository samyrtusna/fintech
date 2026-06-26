using AutoMapper;
using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Persistence;
using fintech.API.Application.Interfaces.Services.IFinancialIntelligenceSnapshotServices;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.Services.FinancialIntelligenceSnapshotServices
{
    public class FinancialIntelligenceSnapshotService( IUnitOfWork unitOfWork, IMapper mapper) : IFinancialIntelligenceSnapshotService
    {
        public async Task<FinancialIntelligenceSnapshotRequestDto> ComputeDailyFinancialIntelligenceSnapshot(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("Date cannot be empty ", nameof(date));
            }
            var transactions = await unitOfWork.FinancialTransactions.GetByDayAsync(userId, date, cancellationToken);
            var lastSnapshot = await unitOfWork.FinancialIntelligenceSnapshots.GetLatestDailyAsync(userId, cancellationToken);

            var groupedByType = transactions.GroupBy(t => t.Type)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            groupedByType.TryGetValue(FinancialType.Income, out var totalIncome);
            groupedByType.TryGetValue(FinancialType.Expense, out var ordinaryExpenses);
            groupedByType.TryGetValue(FinancialType.InterestPayment, out var totalPaidInterests);
            var totalExpenses = ordinaryExpenses + totalPaidInterests;
            var netBalance = totalIncome - totalExpenses;
            groupedByType.TryGetValue(FinancialType.Investment, out var totalInvestments);
            groupedByType.TryGetValue(FinancialType.Savings, out var netSavings);
            groupedByType.TryGetValue(FinancialType.ContractedLoan, out var totalContractedDebts);
            groupedByType.TryGetValue(FinancialType.PrincipalRepayment, out var totalRepayedDebts);
            var cashFlow = netBalance - totalInvestments - totalContractedDebts + totalRepayedDebts;
            var totalNetCashFlow = lastSnapshot != null ? lastSnapshot.TotalNetCashFlow + cashFlow : cashFlow;
            var totalEssentialExpenses = transactions.Where(t => t.Type == FinancialType.Expense && t.IsEssential).Sum(t => t.Amount);
            var topSpendingCategory = await unitOfWork.FinancialTransactions.GetTopSpendingCategoryPerDayAsync(userId, date, cancellationToken);
            var transactionsCount = transactions.Count();
            var averageTransactionAmount = transactionsCount == 0 ? 0 : transactions.Average(t => t.Amount);

            return new FinancialIntelligenceSnapshotRequestDto
            {
                UserId = userId,
                PeriodStart = date,
                PeriodEnd = date.AddDays(1).AddMilliseconds(-1),
                PeriodType = SnapshotPeriodType.Daily,
                TotalIncomes = totalIncome,
                TotalExpenses = totalExpenses,
                NetBalance = netBalance,
                CashFlow = cashFlow,
                TotalNetCashFlow = totalNetCashFlow,
                NetSavings = netSavings,
                TotalInvestments = totalInvestments,
                TotalEssentialExpenses = totalEssentialExpenses,
                TotalContractedDebts = totalContractedDebts,
                TotalRepayedDebts = totalRepayedDebts,
                TotalPaidInterests = totalPaidInterests,
                TopSpendingCategory = topSpendingCategory != null ? topSpendingCategory.Category : "N/A",
                TopSpendingCategoryAmount = topSpendingCategory != null ? topSpendingCategory.TotalAmount : 0,
                TransactionsCount = transactionsCount,
                AvgTransactionAmount = averageTransactionAmount
            };
        }

        // Monthly and yearly snapshots
        public async Task<FinancialIntelligenceSnapshotRequestDto> ComputeMonthlyFinancialIntelligenceSnapshot(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("Date cannot be empty ", nameof(date));
            }
            var snapshots = await unitOfWork.FinancialIntelligenceSnapshots.GetMonthlyAsync(userId, date, cancellationToken)?? throw new NotFoundException($"Financial intelligence snapshots of month {date.Month}-{date.Year} for user {userId} not found");
            var topSpendingCategory = await unitOfWork.FinancialTransactions.GetTopSpendingCategoryPerMonthAsync(userId, date, cancellationToken) ?? throw new NotFoundException($"Top spending Category of month {date.Month}-{date.Year} for user {userId} not found");
            var periodType = SnapshotPeriodType.Monthly;
         
            return GeneratePeriodicFinancialIntelligenceSnapshot(snapshots, topSpendingCategory, periodType);
        }

        public async Task<FinancialIntelligenceSnapshotRequestDto> ComputeYearlyFinancialIntelligenceSnapshot(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (date == DateTime.MinValue)
            {
                throw new ArgumentException("Date cannot be empty ", nameof(date));
            }
            var snapshots = await unitOfWork.FinancialIntelligenceSnapshots.GetYearlyAsync(userId, date, cancellationToken) ?? throw new NotFoundException($"Financial intelligence snapshots of year {date.Year} for user {userId} not found");
            var topSpendingCategory = await unitOfWork.FinancialTransactions.GetTopSpendingCategoryPerYearAsync(userId, date, cancellationToken) ?? throw new NotFoundException($"Top spending Category of year {date.Year} for user {userId} not found");
            var periodType = SnapshotPeriodType.Yearly;

            return GeneratePeriodicFinancialIntelligenceSnapshot(snapshots, topSpendingCategory, periodType);
        }

        public async Task<FinancialIntelligenceSnapshotResponseDto> CreateFinancialIntelligenceSnapshot(FinancialIntelligenceSnapshotRequestDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var snapshotEntity = mapper.Map<FinancialIntelligenceSnapshot>(dto);

            await unitOfWork.FinancialIntelligenceSnapshots.AddAsync(snapshotEntity, cancellationToken);

            var responseDto = mapper.Map<FinancialIntelligenceSnapshotResponseDto>(snapshotEntity);
            return responseDto;
        }

        private static FinancialIntelligenceSnapshotRequestDto GeneratePeriodicFinancialIntelligenceSnapshot(IEnumerable<FinancialIntelligenceSnapshot> snapshots,
            TopSpendingCategoryDto topSpendingCategory,
            SnapshotPeriodType periodType)
        {
            ArgumentNullException.ThrowIfNull(snapshots);
            ArgumentNullException.ThrowIfNull(topSpendingCategory);

            var lastSnapshot = snapshots.MaxBy(s => s.PeriodStart);
            var date = lastSnapshot!.PeriodStart.Date;

            return new FinancialIntelligenceSnapshotRequestDto
            {
                UserId = lastSnapshot!.UserId,
                PeriodStart = date,
                PeriodEnd = date.AddMonths(1).AddDays(-1),
                PeriodType = periodType,
                TotalIncomes = snapshots.Sum(s => s.TotalIncomes),
                TotalExpenses = snapshots.Sum(s => s.TotalExpenses),
                NetBalance = snapshots.Sum(s => s.NetBalance),
                CashFlow = snapshots.Sum(s => s.CashFlow),
                TotalNetCashFlow = lastSnapshot!.TotalNetCashFlow,
                NetSavings = snapshots.Sum(s => s.NetSavings),
                TotalInvestments = snapshots.Sum(e => e.TotalInvestments),
                TotalEssentialExpenses = snapshots.Sum(e => e.TotalEssentialExpenses),
                TotalContractedDebts = snapshots.Sum(e => e.TotalContractedDebts),
                TotalRepayedDebts = snapshots.Sum(s => s.TotalRepayedDebts),
                TotalPaidInterests = snapshots.Sum(s => s.TotalPaidInterests),
                TopSpendingCategory = topSpendingCategory != null ? topSpendingCategory.Category : "N/A",
                TopSpendingCategoryAmount = topSpendingCategory != null ? topSpendingCategory.TotalAmount : 0,
                TransactionsCount = snapshots.Sum(s => s.TransactionsCount),
                AvgTransactionAmount = snapshots.Average(s => s.AvgTransactionAmount)
            };
        }

        // Implement another method for retrieving snapshots if needed
    }
}
