using fintech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.Infrastructure.EFcore.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.PasswordHash)
                .IsRequired();
            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(u => u.BaseCurrency)
                .HasMaxLength(3)
                .IsRequired();
            builder.Property(u => u.CreatedAt)
                .IsRequired ();
            builder.HasIndex(u => u.Email)
                .IsUnique();
            builder.HasIndex(u => u.Username)
                .IsUnique();
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
