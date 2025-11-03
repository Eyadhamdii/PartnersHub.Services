using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.EntityConfigurations;

public class InfraRequestItemFinancialDistributionConfiguration : IEntityTypeConfiguration<InfraRequestItemFinancialDistribution> {
    public void Configure(EntityTypeBuilder<InfraRequestItemFinancialDistribution> builder) {
        builder.HasKey(fd => fd.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(fd => fd.RequestItemId)
            .IsRequired();

        builder.Property(fd => fd.Year)
            .IsRequired();

        builder.Property(fd => fd.AmountType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(fd => fd.Amount)
            .HasConversion(
                amount => amount.Value,
                value => YearlyAmount.Create(value).Value!)
            .HasPrecision(18, 2)
            .IsRequired();

        // Indexes
        builder.HasIndex(fd => fd.RequestItemId);
        builder.HasIndex(fd => fd.Year);

        builder.ToTable("InfraRequestItemFinancialDistributions");
    }
}
