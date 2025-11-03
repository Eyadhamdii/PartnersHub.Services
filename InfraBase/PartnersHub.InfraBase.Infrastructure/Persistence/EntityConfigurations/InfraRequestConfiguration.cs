using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.EntityConfigurations;

public class InfraRequestConfiguration : IEntityTypeConfiguration<InfraRequest> {
    public void Configure(EntityTypeBuilder<InfraRequest> builder) {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RequestCode)
            .HasMaxLength(50);

        builder.Property(r => r.ProjectName)
            .HasConversion(
                name => name.Value,
                value => ProjectName.Create(value).Value!)
            .HasMaxLength(ProjectName.MaxLength)
            .IsRequired();

        builder.Property(r => r.ProjectDescription)
            .HasConversion(
                desc => desc != null ? desc.Value : null,
                value => ProjectDescription.Create(value).Value)
            .HasMaxLength(ProjectDescription.MaxLength);

        builder.Property(r => r.SectorId)
            .IsRequired();

        builder.Property(r => r.SubSectorId)
            .IsRequired();

        builder.Property(r => r.AssetTypeId)
            .IsRequired();

        builder.Property(r => r.AssetTypeOtherDescription)
            .HasMaxLength(500);

        builder.Property(r => r.TenderingStage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.FundingModel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Construction Details
        builder.Property(r => r.StartConstructionQuarter);
        builder.Property(r => r.StartConstructionYear);
        builder.Property(r => r.EndConstructionQuarter);
        builder.Property(r => r.EndConstructionYear);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // TotalRequestAmount is computed - no database column needed
        builder.Ignore(r => r.TotalRequestAmount);

        builder.Property(r => r.CreatedBy)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.RejectionReason)
            .HasConversion(
                reason => reason != null ? reason.Value : null,
                value => value != null ? RejectionReason.Create(value).Value : null)
            .HasMaxLength(RejectionReason.MaxLength);

        builder.Property(r => r.CompanyId);
        
        builder.Property(r => r.CompanyName)
            .HasMaxLength(200);

        // Ignore domain events collection
        builder.Ignore(r => r.DomainEvents);

        // Add RowVersion for optimistic concurrency (SQL Server uses xmin/xmax pattern)
        // This ensures that concurrent modifications are detected properly
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        // Configure relationships for entities within THIS aggregate only
        builder.HasMany(r => r.Items)
            .WithOne()
            .HasForeignKey("RequestId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.History)
            .WithOne()
            .HasForeignKey("RequestId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure attachments relationship
        builder.HasMany(r => r.Attachments)
            .WithOne()
            .HasForeignKey("RequestId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("InfraRequests");
    }
}
