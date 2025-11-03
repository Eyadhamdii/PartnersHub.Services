using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.EntityConfigurations;

public class InfraRequestHistoryConfiguration : IEntityTypeConfiguration<InfraRequestHistory> {
    public void Configure(EntityTypeBuilder<InfraRequestHistory> builder) {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.RequestId)
            .IsRequired();

        builder.Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.Comments)
            .HasMaxLength(3000);

        builder.Property(h => h.PerformedBy)
            .IsRequired();

        builder.Property(h => h.PerformedAt)
            .IsRequired();

        builder.Property(h => h.FieldsChanged)
            .HasMaxLength(1000);

        builder.Property(h => h.OldValues)
            .HasMaxLength(4000);

        builder.Property(h => h.NewValues)
            .HasMaxLength(4000);

        // Index for querying history by request
        builder.HasIndex(h => h.RequestId);

        builder.ToTable("InfraRequestHistories");
    }
}
