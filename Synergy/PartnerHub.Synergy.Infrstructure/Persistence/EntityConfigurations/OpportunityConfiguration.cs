using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.ValueObjects;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// EF Core Configuration for the Opportunity Aggregate Root.
/// </summary>
public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        // 1. Table and Primary Key
        builder.ToTable("Opportunities");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        // 2. Value Objects (Owned Types)

        // Title Value Object
        builder.OwnsOne(o => o.Title, title =>
        {
            title.Property(t => t.Value)
                 .HasColumnName("Title")
                 .IsRequired()
                 .HasMaxLength(Title.MaxLength); // MaxLength is 300
        });

        // Description Value Object
        builder.OwnsOne(o => o.Description, description =>
        {
            description.Property(d => d.Value)
                       .HasColumnName("Description")
                       .HasMaxLength(Description.MaxLength); // MaxLength is 5000
        });
        // Description Value Object
        builder.OwnsOne(o => o.Sector, sector =>
        {
            sector.Property(d => d.Value)
                       .HasColumnName("SectorName");
            sector.Property(d => d.Id)
                       .HasColumnName("SectorId");
        });
        //RepresentativeInformation value object
        builder.OwnsOne(o => o.RepresentativeInformation, tepresentativeInformation =>
        {
            tepresentativeInformation.Property(t => t.Name)
                 .HasColumnName("Name")
                 .IsRequired()
                 .HasMaxLength(Title.MaxLength); // MaxLength is 300
            tepresentativeInformation.Property(t => t.Email)
                 .HasColumnName("Email")
                 .IsRequired()
                 .HasMaxLength(Title.MaxLength); // MaxLength is 300
            tepresentativeInformation.Property(t => t.Phone)
                 .HasColumnName("Phone")
                 .IsRequired()
                 .HasMaxLength(Title.MaxLength); // MaxLength is 300
            tepresentativeInformation.Property(t => t.Position)
                 .HasColumnName("Position")
                 .IsRequired()
                 .HasMaxLength(Title.MaxLength); // MaxLength is 300
        });

        builder.HasOne(o => o.OpportunityType)
               .WithMany()
               .HasForeignKey(o => o.OpportunityTypeId);

        builder.HasOne(o => o.ThematicArea)
               .WithMany()
               .HasForeignKey(o => o.ThematicAreaId);

        builder.HasMany(o => o.Attachments)
               .WithOne()
               .HasForeignKey(a => a.OpportunityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Attachments)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(o => o.CollaboratedCompanies)
               .WithOne()
               .HasForeignKey(osc => osc.OpportunityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.CollaboratedCompanies)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // 3. Scalar Properties and Constraints

        // OpportunityStatus Enum mapping (store as string for readability)
        builder.Property(o => o.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        // Other string properties with assumed max lengths
        builder.Property(o => o.RejectionReason).HasMaxLength(500);
        builder.Property(o => o.CollaborationRationale).HasMaxLength(1000);
        builder.Property(o => o.CollaborationRequirementOther).HasMaxLength(1000);
        builder.Property(o => o.ExpectedOutcomeOther).HasMaxLength(1000);

        // DateOnly properties (EF Core 6+ supports DateOnly/TimeOnly natively)
        builder.Property(o => o.StartDate).IsRequired();
        builder.Property(o => o.EndDate).IsRequired();

        // Terms and Conditions
        builder.Property(o => o.TermsAndConditionId).IsRequired();
        builder.Property(o => o.TermsAccepted).IsRequired();

        // Audit and Approval properties are configured implicitly or in a base config, 
        // but explicit configuration ensures correctness.
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.CreatedBy).IsRequired();
        // Nullable properties (RejectedBy, ApprovedBy, UpdatedAt, etc.) are configured by omitting IsRequired()

        // 4. Collections (Many-to-Many Relationships)


        builder.Navigation(o => o.CollaborationRequirements)
               .UsePropertyAccessMode(PropertyAccessMode.Field); // <-- Use private backing field



        builder.Navigation(o => o.ExpectedOutcomes)
               .UsePropertyAccessMode(PropertyAccessMode.Field); // <-- Use private backing field
    }
}