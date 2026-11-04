using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
    {
        public void Configure (EntityTypeBuilder<FinancialTransaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.UserId)
                .IsRequired();
            builder.Property(t => t.CategoryId)
                .IsRequired();
            builder.Property(c => c.Type)
               .HasConversion<string>()
               .IsRequired()
               .HasMaxLength(20);
            builder.OwnsOne(t => t.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2);
                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                        .HasColumnName("CurrencyCode")
                        .HasMaxLength(3)
                        .IsRequired();

                    currency.Property(c => c.Symbol)
                        .HasColumnName("CurrencySymbol")
                        .HasMaxLength(3)
                        .IsRequired();
                });
                
            });
            builder.Property(t => t.ExchangeRate)
                .IsRequired()
                .HasColumnType("decimal(18,6)");
            builder.OwnsOne(t => t.BaseAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("BaseAmount")
                    .HasPrecision(18, 2);
                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                        .HasColumnName("BaseCurrencyCode")
                        .HasMaxLength(3)
                        .IsRequired();

                    currency.Property(c => c.Symbol)
                        .HasColumnName("BaseCurrencySymbol")
                        .HasMaxLength(3)
                        .IsRequired();
                });              
            });
            builder.Property(t => t.Description)
                .HasMaxLength(30);
            builder.Property(t => t.IsEssential)
                .IsRequired();
            builder.Property(t => t.TransactionDate)
                .IsRequired();
            builder.Property(t => t.CreatedAt)
                .IsRequired();  
            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false);
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);
        }
    }
}
