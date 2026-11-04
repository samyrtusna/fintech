using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class FinancialAggregatesConfiguration : IEntityTypeConfiguration<FinancialAggregate>
    {
        public void Configure(EntityTypeBuilder<FinancialAggregate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.OwnsOne(x => x.Period, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("PeriodStartDate")
                    .IsRequired();

                period.Property(p => p.EndDate)
                    .HasColumnName("PeriodEndDate")
                    .IsRequired();
            });
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
        }
    }
}
