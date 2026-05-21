using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class FinancialIntelligenceSnapshotConfiguration : IEntityTypeConfiguration<FinancialIntelligenceSnapshot>
    {
        public void Configure (EntityTypeBuilder<FinancialIntelligenceSnapshot> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.UserId)
                .IsRequired();
            builder.Property(f => f.PeriodStart)
                .IsRequired();
            builder.Property(f => f.PeriodEnd)
                .IsRequired();
            builder.Property(c => c.PeriodType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(f => f.TotalIncomes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalExpenses)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.NetBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.CashFlow)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalNetCashFlow)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.NetSavings)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalEssentialExpenses)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalInvestments)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalContractedDebts)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TotalRepayedDebts)
                .HasColumnType("decimal(18,2)")
                .IsRequired(); 
            builder.Property(f => f.TotalPaidInterests)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TopSpendingCategory)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(f => f.TopSpendingCategoryAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.TransactionsCount)
                .IsRequired();
            builder.Property(f => f.AvgTransactionAmount)
                .HasColumnType("decimal(18,2)");
            builder.HasMany(f => f.FinancialIndicators)
                .WithOne(fi => fi.SnapShot)
                .HasForeignKey(fi => fi.SnapshotId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
