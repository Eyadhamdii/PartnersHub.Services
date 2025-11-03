using ConfigurationHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.ConfigurationHub.Domain;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.EntityConfigurations;

public class WhiteListIPConfiguration : IEntityTypeConfiguration<WhiteListIP> {
    public void Configure(EntityTypeBuilder<WhiteListIP> builder) {
        builder.HasKey(p => p.Id);
        builder.OwnsOne(p => p.IPAddress, ip => {
            ip.Property(i => i.Value)
            .HasColumnName("IPAddress")
            .HasMaxLength(64)
            .IsRequired();
        });
        //builder.Property(p => p.IPAddress)
        //    .HasConversion(
        //        address => address.Value,
        //        value => IPAddress.Create(value))
        //    .HasMaxLength(64)
        //    .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();




    }
}