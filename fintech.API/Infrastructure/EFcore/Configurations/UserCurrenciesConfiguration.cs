using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class UserCurrenciesConfiguration : IEntityTypeConfiguration<UserCurrency>
    {
        public void Configure(EntityTypeBuilder<UserCurrency> builder)
        {
            builder.HasKey(uc => uc.Id);
            builder.OwnsOne(uc => uc.Currency, currency =>
            {
                currency.Property(c => c.Code)
                   .HasColumnName("CurrencyCode")
                   .HasMaxLength(3)
                   .IsRequired();

                currency.Property(c => c.Symbol)
                    .HasColumnName("CurrencySymbol")
                    .HasMaxLength(10)
                    .IsRequired();
            });
                
            builder.Property(uc => uc.IsDefault)
                .IsRequired();
            builder.Property(uc => uc.IsDeleted)
                .IsRequired();
            builder.HasOne(uc => uc.User)
                .WithMany(u => u.UserCurrencies)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
