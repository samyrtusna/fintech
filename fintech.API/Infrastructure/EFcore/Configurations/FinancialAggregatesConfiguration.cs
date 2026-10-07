using fintech.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fintech.API.Infrastructure.EFcore.Configurations
{
    public class FinancialAggregatesConfiguration : IEntityTypeConfiguration<FinancialAggregate>
    {
        public void Configure(EntityTypeBuilder<FinancialAggregate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Value).HasColumnType("decimal(18,2)");
            builder.HasIndex(x => x.Year);
        }
    }
}
