using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.EntityConfigurations;

public class InfraRequestItemConfiguration : IEntityTypeConfiguration<InfraRequestItem> {
    public void Configure(EntityTypeBuilder<InfraRequestItem> builder) {
        builder.HasKey(i => i.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        // Shadow property for foreign key
        builder.Property<Guid>("RequestId")
            .IsRequired();

        builder.Property(i => i.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.ItemName)
            .HasConversion(
                name => name.Value,
                value => ItemName.Create(value).Value!)
            .HasMaxLength(ItemName.MaxLength)
            .IsRequired();

        builder.Property(i => i.UomId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasConversion(
                qty => qty.Value,
                value => Quantity.Create(value).Value!)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasConversion(
                price => price.Value,
                value => UnitPrice.Create(value).Value!)
            .HasPrecision(18, 2)
            .IsRequired();

        // TotalAmount is computed - no database column needed
        builder.Ignore(i => i.TotalAmount);

        // Configure relationships within aggregate
        builder.HasMany(i => i.FinancialDistributions)
            .WithOne()
            .HasForeignKey("RequestItemId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex("RequestId");
        builder.HasIndex(i => i.ItemCode);

        builder.ToTable("InfraRequestItems");
    }
}
