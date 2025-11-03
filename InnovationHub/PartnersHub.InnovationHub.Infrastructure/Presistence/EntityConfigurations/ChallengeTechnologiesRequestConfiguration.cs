using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using PartnersHub.InnovationHub.Domain.Common; // Adjust namespace as necessary
// Assuming your entities (Technology, RequestStatus) and configurations 
// (AggregateRootConfiguration) are accessible.

public class ChallengeTechnologiesRequestConfiguration : AuditableEntityConfiguration<ChallengeTechnologiesRequest>
{
    public override void Configure(EntityTypeBuilder<ChallengeTechnologiesRequest> builder)
    {
        // 1. Apply base configuration (Id, CreatedAt/By, UpdatedAt/By, and ignores DomainEvents)
        base.Configure(builder);

        builder.Property(r => r.ChallengeRequestId)
               .IsRequired();

        builder.HasOne(r => r.LinkedTechnology)
               .WithMany()
               .HasForeignKey(r => r.TechnologyId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.TechnologyId)
               .IsRequired();

        builder.Property(r => r.JustificationForLinking)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(r => r.RequestStatus)
               .IsRequired()
               .HasConversion<string>();

        builder.HasIndex(r => new { r.ChallengeRequestId, r.TechnologyId })
               .IsUnique();

    }
}