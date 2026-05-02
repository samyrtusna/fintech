using fintech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.Infrastructure.EFcore.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
    {
        public void Configure (EntityTypeBuilder<FinancialTransaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.UserId)
                .IsRequired();
            builder.Property(t => t.CategoryId)
                .IsRequired();
            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(t => t.ExchangeRate)
                .IsRequired()
                .HasColumnType("decimal(18,6)");
            builder.Property(t => t.BaseAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(t => t.Description)
                .HasMaxLength(500);
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
