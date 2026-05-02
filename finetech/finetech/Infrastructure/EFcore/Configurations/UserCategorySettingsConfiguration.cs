using fintech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.Infrastructure.EFcore.Configurations
{
    public class UserCategorySettingsConfiguration : IEntityTypeConfiguration<UserCategorySetting>
    {
        public void Configure(EntityTypeBuilder<UserCategorySetting> builder)
        {
            builder.HasKey(ucs => ucs.Id);
            builder.Property(ucs => ucs.IsEssential)
                .IsRequired();
            builder.HasOne(ucs => ucs.Category)
                .WithMany()
                .HasForeignKey(ucs => ucs.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(ucs => new { ucs.UserId, ucs.CategoryId })
                .IsUnique();
        }
    }
}
