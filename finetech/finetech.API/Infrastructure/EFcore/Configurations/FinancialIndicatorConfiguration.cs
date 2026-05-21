using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class FinancialIndicatorConfiguration : IEntityTypeConfiguration<FinancialIndicator>
    {
        public void Configure (EntityTypeBuilder<FinancialIndicator> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.UserId)
                .IsRequired();
            builder.Property(f => f.SnapshotId)
                .IsRequired();
            builder.Property(f => f.PeriodStart)
                .IsRequired();
            builder.Property(f => f.PeriodEnd)
                .IsRequired();
            builder.Property(c => c.PeriodType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(f => f.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(f => f.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(f => f.Description)
                .HasMaxLength(500)
                .IsRequired();
            builder.HasOne(f => f.SnapShot)
                .WithMany(s => s.FinancialIndicators)
                .HasForeignKey(f => f.SnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
