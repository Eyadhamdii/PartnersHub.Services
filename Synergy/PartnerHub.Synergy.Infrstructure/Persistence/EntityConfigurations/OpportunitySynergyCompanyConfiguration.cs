using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using System.Reflection.Emit;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations;

public class OpportunitySynergyCompanyConfiguration : IEntityTypeConfiguration<OpportunitySynergyCompany>
{
    public void Configure(EntityTypeBuilder<OpportunitySynergyCompany> builder)
    {
        builder.ToTable("OpportunitySynergyCompanies");
        builder.HasKey(osc => osc.Id);
        builder.Property(osc => osc.Id).ValueGeneratedNever();

        builder.Property(osc => osc.OpportunityId).IsRequired();
        builder.Property(osc => osc.SynergyCompanyId).IsRequired();
        builder.Property(osc => osc.CollaborationDate).IsRequired();

        builder.HasIndex(osc => osc.OpportunityId);
        builder.HasIndex(osc => osc.SynergyCompanyId);
        builder.HasIndex(osc => new { osc.OpportunityId, osc.SynergyCompanyId }).IsUnique();

    }
}
