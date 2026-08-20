using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class OptionsConfiguration : IEntityTypeConfiguration<Options>
    {
        public void Configure(EntityTypeBuilder<Options> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Key)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(o => o.Value)
                .IsRequired();
        }
    }
}
