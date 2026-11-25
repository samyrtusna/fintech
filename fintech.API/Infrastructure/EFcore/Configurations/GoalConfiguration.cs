using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class GoalConfiguration : IEntityTypeConfiguration<Goals>
    {
        public void Configure(EntityTypeBuilder<Goals> builder)
        {
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);
            builder.OwnsOne(t => t.TargetAmount, money =>
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
            builder.Property(g => g.TargetType)
                .HasConversion<string>()
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
            builder.Property(g => g.GoalProgress)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.Property(g => g.IsActive)
                .IsRequired();
            builder.Property(g => g.IsDeleted)
                .IsRequired();
            builder.Property(g => g.CreatedAt)
                .IsRequired();
            builder.HasOne(g => g.Category)
                .WithMany()
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
