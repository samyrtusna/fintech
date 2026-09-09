using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Token)
                .IsRequired();
            builder.Property(r => r.ExpiresAt)
                .IsRequired();
            builder.Property(r => r.IsRevoked)
                .IsRequired();
            builder.Property(r => r.CreatedAt)
                .IsRequired();
            builder.Property(r => r.ReplacedByToken);
        }
    }
}
