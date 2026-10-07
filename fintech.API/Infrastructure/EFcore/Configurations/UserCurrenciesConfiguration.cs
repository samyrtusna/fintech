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
            builder.Property(uc => uc.CurrencySymbol)
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(uc => uc.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);
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
