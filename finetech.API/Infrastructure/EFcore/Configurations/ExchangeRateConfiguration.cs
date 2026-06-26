using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure (EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(er => er.Id);
            builder.Property(er => er.FromCurrency)
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(er => er.ToCurrency)
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(er => er.Rate)
                .IsRequired()
                .HasColumnType("decimal(18,6)");
            builder.Property(er => er.Date)
                .IsRequired();
            builder.HasIndex(er => new { er.FromCurrency, er.ToCurrency, er.Date })
                .IsUnique();
        }
    }
}
