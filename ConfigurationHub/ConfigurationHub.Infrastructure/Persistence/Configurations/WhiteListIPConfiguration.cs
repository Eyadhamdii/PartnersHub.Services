using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Configurations;

public class WhiteListIPConfiguration : IEntityTypeConfiguration<WhiteListIP> {
    public void Configure(EntityTypeBuilder<WhiteListIP> builder) {
        builder.ToTable("WhiteListIPs");

        builder.HasKey(w => w.Id);

        builder.OwnsOne(w => w.IPAddress, ip => {
            ip.Property(i => i.Value)
                .HasColumnName("IPAddress")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(w => w.ExpiryDate)
            .IsRequired();

        builder.Property(w => w.IsActive)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.CreatedBy)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.HasIndex(w => w.IsActive);
        builder.HasIndex(w => w.ExpiryDate);
    }
}