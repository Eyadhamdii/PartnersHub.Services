using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations;

public class SynergyCompanySectorConfiguration : IEntityTypeConfiguration<SynergyCompanySector>
{
    public void Configure(EntityTypeBuilder<SynergyCompanySector> builder)
    {
        builder.ToTable("SynergyCompanySectors");
        builder.HasKey(cs => cs.Id);
        builder.Property(cs => cs.Id).ValueGeneratedNever();

        builder.Property(cs => cs.CompanyId).IsRequired();
        builder.Property(cs => cs.SectorId).IsRequired();
        builder.Property(cs => cs.SectorName).IsRequired().HasMaxLength(200);
        builder.Property(cs => cs.AssignedDate).IsRequired();

        builder.HasIndex(cs => cs.CompanyId);
        builder.HasIndex(cs => cs.SectorId);
        builder.HasIndex(cs => new { cs.CompanyId, cs.SectorId }).IsUnique();
    }
}
