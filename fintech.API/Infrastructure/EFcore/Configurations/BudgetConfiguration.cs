using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(b => b.Id);
            builder.OwnsOne(t => t.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2);
                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                        .HasColumnName("CurrencyCode")
                        .HasMaxLength(3);

                    currency.Property(c => c.Symbol)
                        .HasColumnName("CurrencySymbol")
                        .HasMaxLength(3);
                });

            });
            builder.Property(b => b.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            builder.OwnsOne(x => x.Period, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("PeriodStartDate")
                    .IsRequired();

                period.Property(p => p.EndDate)
                    .HasColumnName("PeriodEndDate")
                    .IsRequired();
            });
            builder.Property(b => b.IsActive)
                .IsRequired();
            builder.Property(b => b.CreatedAt)
                .IsRequired();
            builder.HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
